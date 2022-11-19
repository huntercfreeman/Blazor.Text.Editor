using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class TextEditorJavaScriptDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (JavaScriptDecorationKind)decorationByte;

        return decoration switch
        {
            JavaScriptDecorationKind.None => string.Empty,
            JavaScriptDecorationKind.Keyword => "bte_keyword",
            _ => string.Empty,
        };
    }
}
