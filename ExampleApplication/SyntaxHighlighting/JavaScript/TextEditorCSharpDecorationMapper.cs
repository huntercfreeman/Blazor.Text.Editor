using BlazorTextEditor.RazorLib.Decoration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExampleApplication.SyntaxHighlighting.JavaScript;

public class TextEditorJavaScriptDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (DecorationKind)decorationByte;

        return decoration switch
        {
            DecorationKind.None => string.Empty,
            DecorationKind.Method => "bte_method",
            DecorationKind.Type => "bte_type",
            DecorationKind.Parameter => "bte_parameter",
            DecorationKind.StringLiteral => "bte_string-literal",
            DecorationKind.Keyword => "bte_keyword",
            DecorationKind.Comment => "bte_comment",
            _ => throw new ApplicationException(
                $"The {nameof(DecorationKind)}: {decoration} was not recognized.")
        };
    }
}