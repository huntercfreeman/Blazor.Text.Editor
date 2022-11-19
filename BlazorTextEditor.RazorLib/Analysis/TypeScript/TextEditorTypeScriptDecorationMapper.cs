using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript;

public class TextEditorTypeScriptDecorationMapper : IDecorationMapper
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
