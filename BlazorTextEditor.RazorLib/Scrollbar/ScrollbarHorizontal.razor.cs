using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Scrollbar;

public partial class ScrollbarHorizontal : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    
    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);
    
    private bool _thinksLeftMouseButtonIsDown;

    private readonly Guid _scrollbarGuid = Guid.NewGuid();
    
    private string ScrollbarElementId => $"bte_{_scrollbarGuid}";
    
    private string GetScrollbarHorizontalStyleCss()
    {
        var gutterWidthInPixels = GetGutterWidthInPixels();
        
        var left = $"left: {gutterWidthInPixels}px;";

        var width = $"width: calc(100% - {gutterWidthInPixels}px - {ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS}px);";

        return $"{left} {width}";
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
        var scrollbarWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels - 
                                     ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS - 
                                     GetGutterWidthInPixels();
        
        // Proportional Left
        var sliderProportionalLeftInPixels = VirtualizationResult.VirtualizationScrollPosition.ScrollLeftInPixels *
                                             scrollbarWidthInPixels /
                                            VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels;

        var left = $"left: {sliderProportionalLeftInPixels}px;";
        
        // Proportional Width
        var sliderProportionalWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels *
                                               scrollbarWidthInPixels /
                                               VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels;

        var width = $"width: {sliderProportionalWidthInPixels}px;";
        
        return $"{left} {width}";
    }
    
    private Task HandleOnMouseDownAsync(MouseEventArgs arg)
    {
        _thinksLeftMouseButtonIsDown = true;

        return Task.CompletedTask;
    }
    
    private async Task HandleOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
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
                (mouseEventArgs.Buttons & 1) == 1)
            {
                var relativeCoordinatesOnClick = await JsRuntime
                    .InvokeAsync<RelativeCoordinates>(
                        "blazorTextEditor.getRelativePosition",
                        ScrollbarElementId,
                        mouseEventArgs.ClientX,
                        mouseEventArgs.ClientY);
                
                var scrollbarWidthInPixels = WidthAndHeightOfTextEditor.WidthInPixels - 
                                              ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

                var scrollLeft = relativeCoordinatesOnClick.RelativeX *
                                VirtualizationResult.VirtualizationScrollPosition.ScrollWidthInPixels /
                                scrollbarWidthInPixels;

                await TextEditorDisplay.SetScrollPositionAsync(scrollLeft, null);
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
}