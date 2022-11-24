using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Scrollbar;

public partial class ScrollbarHorizontal : ComponentBase
{
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    
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
}