using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class GutterSection : ComponentBase
{
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    
    private string GetGutterStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        var top =
            $"top: {index * CharacterWidthAndRowHeight.RowHeightInPixels}px;";
        var height =
            $"height: {CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditor.RowCount
            .ToString()
            .Length;

        var widthInPixels = mostDigitsInARowLineNumber *
                            CharacterWidthAndRowHeight.CharacterWidthInPixels;

        widthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var width = $"width: {widthInPixels}px;";

        var paddingLeft =
            $"padding-left: {TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS}px;";
        var paddingRight =
            $"padding-right: {TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS}px;";

        var left = $"left: {virtualizedRowLeftInPixels}px;";

        return $"{left} {top} {height} {width} {paddingLeft} {paddingRight}";
    }
}