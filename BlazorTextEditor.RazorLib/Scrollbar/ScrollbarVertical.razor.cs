using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Scrollbar;

public partial class ScrollbarVertical : ComponentBase
{
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    
    private string GetSliderVerticalStyleCss()
    {
        // Top
        var sliderProportionalTopInPixels = VirtualizationResult.VirtualizationScrollPosition.ScrollTopInPixels * 
                                            WidthAndHeightOfTextEditor.HeightInPixels /
                                            VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;
              
        var top = $"top: {sliderProportionalTopInPixels}px;";
        
        // Height
        var scrollbarHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        var sliderProportionalHeightInPixels = WidthAndHeightOfTextEditor.HeightInPixels *
                                               scrollbarHeightInPixels /
                                               VirtualizationResult.VirtualizationScrollPosition.ScrollHeightInPixels;

        var height = $"height: {sliderProportionalHeightInPixels}px;";
        
        return $"{top} {height}";
    }
}