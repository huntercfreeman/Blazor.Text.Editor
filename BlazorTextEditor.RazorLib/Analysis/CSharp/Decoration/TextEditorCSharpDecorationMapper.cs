using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;

public class TextEditorCSharpDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (CSharpDecorationKind)decorationByte;

        return decoration switch
        {
            CSharpDecorationKind.None => string.Empty,
            CSharpDecorationKind.Method => "bte_method",
            CSharpDecorationKind.Type => "bte_type",
            CSharpDecorationKind.Parameter => "bte_parameter",
            CSharpDecorationKind.StringLiteral => "bte_string-literal",
            CSharpDecorationKind.Keyword => "bte_keyword",
            CSharpDecorationKind.Comment => "bte_comment",
            _ => string.Empty,
        };
    }
}