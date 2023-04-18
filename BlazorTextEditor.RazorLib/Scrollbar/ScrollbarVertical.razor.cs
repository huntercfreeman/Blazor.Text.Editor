using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.JavaScriptObjects;
using BlazorCommon.RazorLib.Reactive;
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

public partial class ScrollbarVertical : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    // TODO: The ValueTuple being used here needs to be made into a class likely as this is not nice to read
    private readonly IThrottle<((MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), bool thinksLeftMouseButtonIsDown)> 
        _onMouseMoveThrottle = 
            new Throttle<((MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), bool thinksLeftMouseButtonIsDown)>(
                TimeSpan.FromMilliseconds(25));
    
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
    
    private string GetSliderVerticalStyleCss()
    {
        var scrollbarHeightInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height - 
                                      ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Top
        var sliderProportionalTopInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollTop *
                                            scrollbarHeightInPixels /
                                            TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight;
        
        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels
            .ToCssValue();
              
        var top = $"top: {sliderProportionalTopInPixelsInvariantCulture}px;";
        
        // Proportional Height
        var pageHeight = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
        
        var sliderProportionalHeightInPixels = pageHeight *
                                               scrollbarHeightInPixels /
                                               TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight;
        
        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels
            .ToCssValue();

        var height = $"height: {sliderProportionalHeightInPixelsInvariantCulture}px;";
        
        return $"{top} {height}";
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
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return;
        
        var mostRecentEventArgs = await _onMouseMoveThrottle.FireAsync(
            (mouseEventArgsTuple, localThinksLeftMouseButtonIsDown),
            CancellationToken.None);

        if (mostRecentEventArgs.isCancellationRequested)
            return;

        localThinksLeftMouseButtonIsDown = mostRecentEventArgs.tEventArgs.thinksLeftMouseButtonIsDown;
        mouseEventArgsTuple = mostRecentEventArgs.tEventArgs.Item1;
    
        // Buttons is a bit flag
            // '& 1' gets if left mouse button is held
            if (localThinksLeftMouseButtonIsDown &&
                (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
            {
                var relativeCoordinates = await JsRuntime
                    .InvokeAsync<RelativeCoordinates>(
                        "blazorTextEditor.getRelativePosition",
                        ScrollbarElementId,
                        mouseEventArgsTuple.secondMouseEventArgs.ClientX,
                        mouseEventArgsTuple.secondMouseEventArgs.ClientY);
             
                var yPosition = Math.Max(0, relativeCoordinates.RelativeY);

                if (yPosition > TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height)
                    yPosition = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
                
                var scrollbarHeightInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height - 
                                              ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
    
                var scrollTop = yPosition *
                                TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight /
                                scrollbarHeightInPixels;
                
                if (scrollTop + TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height > 
                    TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight)
                {
                    scrollTop = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight -
                                TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
                }
    
                await TextEditorViewModel.SetScrollPositionAsync(null, scrollTop);
            }
            else
            {
                _thinksLeftMouseButtonIsDown = false;
            }
    }
    
    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}