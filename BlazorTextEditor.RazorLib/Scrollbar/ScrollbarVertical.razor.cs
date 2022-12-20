using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Scrollbar;

public partial class ScrollbarVertical : ComponentBase
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
    
    private string GetSliderVerticalStyleCss()
    {
        var scrollbarHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels - 
                                      ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Top
        var sliderProportionalTopInPixels = VirtualizationResult.VirtualizationScrollPosition.ScrollTopInPixels *
                                            scrollbarHeightInPixels /
                                            VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;
              
        var top = $"top: {sliderProportionalTopInPixels}px;";
        
        // Proportional Height
        var sliderProportionalHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels *
                                               scrollbarHeightInPixels /
                                               VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;

        var height = $"height: {sliderProportionalHeightInPixels}px;";
        
        return $"{top} {height}";
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
                
                var scrollbarHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels - 
                                              ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

                var scrollTop = relativeCoordinatesOnClick.RelativeY *
                                VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels /
                                scrollbarHeightInPixels;

                await TextEditorDisplay.SetScrollPositionAsync(null, scrollTop);
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