using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorDiffDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TextEditorDiffDecorationKind)decorationByte;

        return decoration switch
        {
            TextEditorDiffDecorationKind.None => string.Empty,
            TextEditorDiffDecorationKind.LongestCommonSubsequence => "bte_diff-longest-common-subsequence-presentation",
            _ => string.Empty,
        };
    }
}