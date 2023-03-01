using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Drag;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Store.DragCase;
using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.JavaScriptObjects;
using BlazorCommon.RazorLib.Store.DragCase;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Scrollbar;

public partial class ScrollbarHorizontal : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);
    
    private bool _thinksLeftMouseButtonIsDown;

    private readonly Guid _scrollbarGuid = Guid.NewGuid();
    
    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;
    
    private string ScrollbarElementId => $"bte_{_scrollbarGuid}";
    
    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }
    
    private string GetScrollbarHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width -
                                  ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels
            .ToCssValue();
        
        var width = $"width: {scrollbarWidthInPixelsInvariantCulture}px;";

        return width;
    }
    
    private string GetSliderHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width -
                                           ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Left
        var sliderProportionalLeftInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollLeft *
                                             scrollbarWidthInPixels /
                                             TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;
        
        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels
            .ToCssValue();

        var left = $"left: {sliderProportionalLeftInPixelsInvariantCulture}px;";
        
        // Proportional Width
        var pageWidth = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
        
        var sliderProportionalWidthInPixels = pageWidth *
                                              scrollbarWidthInPixels /
                                              TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;
        
        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels
            .ToCssValue();

        var width = $"width: {sliderProportionalWidthInPixelsInvariantCulture}px;";
        
        return $"{left} {width}";
    }

    private Task HandleOnMouseDownAsync(MouseEventArgs arg)
    {
        _thinksLeftMouseButtonIsDown = true;
        SubscribeToDragEventForScrolling();

        return Task.CompletedTask;
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null &&
                    mouseEventArgs is not null)
                {
                    await _dragEventHandler.Invoke((_previousDragMouseEventArgs, mouseEventArgs));
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public void SubscribeToDragEventForScrolling()
    {
        _dragEventHandler = DragEventHandlerScrollAsync;
        Dispatcher.Dispatch(new DragState.SetDragStateAction(true, null));
    }
    
    private async Task DragEventHandlerScrollAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        var success = await _onMouseMoveSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);
    
        if (!success)
            return;
    
        try
        {
            // Buttons is a bit flag
            // '& 1' gets if left mouse button is held
            if (_thinksLeftMouseButtonIsDown &&
                (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
            {
                var relativeCoordinates = await JsRuntime
                    .InvokeAsync<RelativeCoordinates>(
                        "blazorTextEditor.getRelativePosition",
                        ScrollbarElementId,
                        mouseEventArgsTuple.secondMouseEventArgs.ClientX,
                        mouseEventArgsTuple.secondMouseEventArgs.ClientY);
                
                var xPosition = Math.Max(0, relativeCoordinates.RelativeX);

                if (xPosition > TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height)
                    xPosition = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
                
                var scrollbarWidthInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width - 
                                             ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

                var scrollLeft = xPosition *
                                 TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth /
                                 scrollbarWidthInPixels;
        
                if (scrollLeft + TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width > 
                    TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth)
                {
                    scrollLeft = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth -
                                 TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
                }
                
                await TextEditorViewModel.SetScrollPositionAsync(scrollLeft, null);
            }
            else
            {
                _thinksLeftMouseButtonIsDown = false;
            }
    
            await Task.Delay(_onMouseMoveDelay);
        }
        finally
        {
            _onMouseMoveSemaphoreSlim.Release();
        }
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}