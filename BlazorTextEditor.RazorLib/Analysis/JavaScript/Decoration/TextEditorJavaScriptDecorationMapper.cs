using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript.Decoration;

public class TextEditorJavaScriptDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (JavaScriptDecorationKind)decorationByte;

        return decoration switch
        {
            JavaScriptDecorationKind.None => string.Empty,
            JavaScriptDecorationKind.Keyword => "bte_keyword",
            JavaScriptDecorationKind.String => "bte_string-literal",
            JavaScriptDecorationKind.Comment => "bte_comment",
            _ => string.Empty,
        };
    }
}
