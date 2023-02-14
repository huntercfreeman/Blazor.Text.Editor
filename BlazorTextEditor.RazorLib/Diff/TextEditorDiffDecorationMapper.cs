using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorDiffDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (CSharpDecorationKind)decorationByte;

        return decoration switch
        {
            CSharpDecorationKind.None => string.Empty,
            CSharpDecorationKind.Method => "bte_diff-match",
            _ => string.Empty,
        };
    }
}