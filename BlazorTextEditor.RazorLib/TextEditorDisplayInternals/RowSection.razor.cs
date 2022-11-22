using System.Text;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class RowSection : ComponentBase
{
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool GlobalShowNewlines { get; set; }
    [Parameter, EditorRequired]
    public string TabKeyOutput { get; set; } = null!;
    [Parameter, EditorRequired]
    public string SpaceKeyOutput { get; set; } = null!;

    private string GetRowStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        var top =
            $"top:{index * CharacterWidthAndRowHeight.RowHeightInPixels}px;";
        var height =
            $"height: {CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditor.RowCount
            .ToString()
            .Length;

        var widthOfGutterInPixels = mostDigitsInARowLineNumber *
                                    CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var leftInPixels = widthOfGutterInPixels +
                           virtualizedRowLeftInPixels +
                           TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                           TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var left = $"left: {leftInPixels}px;";

        return $"{top} {height} {left}";
    }
    
    private string GetCssClass(byte decorationByte)
    {
        return TextEditor.DecorationMapper.Map(decorationByte);
    }
    
    private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }
}