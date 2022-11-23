using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class ScrollbarSection : ComponentBase
{
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;

    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    
    private const double SCROLLBAR_SIZE_IN_PIXELS = 30;
    
    private string GetScrollbarHorizontalStyleCss()
    {
        var gutterWidthInPixels = GetGutterWidthInPixels();
        
        var left = $"left: {gutterWidthInPixels}px;";

        var width = $"width: calc(100% - {gutterWidthInPixels}px - {SCROLLBAR_SIZE_IN_PIXELS}px);";

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
        return string.Empty;
    }

    private string GetSliderVerticalStyleCss()
    {
        // Top
        var sliderProportionalTopInPixels = VirtualizationResult.VirtualizationScrollPosition.ScrollTopInPixels * 
                                    WidthAndHeightOfTextEditor.HeightInPixels /
                                    VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;
              
        var top = $"top: {sliderProportionalTopInPixels}px;";
        
        // Height
        var scrollbarHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels - SCROLLBAR_SIZE_IN_PIXELS;
        var sliderProportionalHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels *
                                               scrollbarHeightInPixels /
                                               VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;

        var height = $"height: {sliderProportionalHeightInPixels}px;";
        
        return $"{top} {height}";
    }
}