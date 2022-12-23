using BlazorALaCarte.Shared.DragCase;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
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
    public TextEditorBase TextEditorBase { get; set; } = null!;
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
    
    private string GetSliderVerticalStyleCss()
    {
        var scrollbarHeightInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height - 
                                      ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Top
        var sliderProportionalTopInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollTop *
                                            scrollbarHeightInPixels /
                                            TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight;
              
        var top = $"top: {sliderProportionalTopInPixels}px;";
        
        // Proportional Height
        var pageHeight = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
        
        var sliderProportionalHeightInPixels = pageHeight *
                                               scrollbarHeightInPixels /
                                               TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight;

        var height = $"height: {sliderProportionalHeightInPixels}px;";
        
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
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
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
             
                var yPosition = Math.Max(0, relativeCoordinates.RelativeY);

                if (yPosition > TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height)
                    yPosition = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height;
                
                var scrollbarHeightInPixels = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Height - 
                                              ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
    
                var scrollTop = yPosition *
                                TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollHeight /
                                scrollbarHeightInPixels;
    
                await TextEditorViewModel.SetScrollPositionAsync(null, scrollTop);
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