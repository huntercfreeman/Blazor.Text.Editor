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

public partial class ScrollbarHorizontal : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
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
        var gutterWidthInPixels = GetGutterWidthInPixels();

        var scrollbarWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels - 
                                  gutterWidthInPixels -
                                  ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        var width = $"width: {scrollbarWidthInPixels}px;";

        return width;
    }
    
    private double GetGutterWidthInPixels()
    {
        var mostDigitsInARowLineNumber = TextEditor.RowCount
            .ToString()
            .Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
                                  CharacterWidthAndRowHeight.CharacterWidthInPixels;

        gutterWidthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                               TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        return gutterWidthInPixels;
    }

    private string GetSliderHorizontalStyleCss()
    {
        var gutterWidthInPixels = GetGutterWidthInPixels();

        var scrollbarWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels - 
                                           gutterWidthInPixels -
                                           ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Left
        var sliderProportionalLeftInPixels = VirtualizationResult.VirtualizationScrollPosition.ScrollLeftInPixels *
                                             scrollbarWidthInPixels /
                                             VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels;

        var left = $"left: {sliderProportionalLeftInPixels}px;";
        
        // Proportional Width
        var pageWidth = WidthAndHeightOfTextEditor.WidthInPixels -
                        gutterWidthInPixels;
        
        var sliderProportionalWidthInPixels = pageWidth *
                                              scrollbarWidthInPixels /
                                              VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels;

        var width = $"width: {sliderProportionalWidthInPixels}px;";
        
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
                
                var xPosition = Math.Max(0, relativeCoordinates.RelativeX);

                if (xPosition > WidthAndHeightOfTextEditor.HeightInPixels)
                    xPosition = WidthAndHeightOfTextEditor.HeightInPixels;
                
                var scrollbarWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels - 
                                             ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

                var scrollLeft = xPosition *
                                 VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels /
                                 scrollbarWidthInPixels;

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