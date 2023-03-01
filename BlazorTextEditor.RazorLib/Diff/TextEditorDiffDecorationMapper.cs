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
            TextEditorDiffDecorationKind.LongestCommonSubsequence => "bte_diff-longest-common-subsequence",
            TextEditorDiffDecorationKind.Insertion => "bte_diff-insertion",
            TextEditorDiffDecorationKind.Deletion => "bte_diff-deletion",
            TextEditorDiffDecorationKind.Modification => "bte_diff-modification",
            _ => string.Empty,
        };
    }
}