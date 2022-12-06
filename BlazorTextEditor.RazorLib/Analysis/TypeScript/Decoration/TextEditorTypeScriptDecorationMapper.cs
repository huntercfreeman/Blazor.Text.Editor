using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript.Decoration;

public class TextEditorTypeScriptDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TypeScriptDecorationKind)decorationByte;

        return decoration switch
        {
            TypeScriptDecorationKind.None => string.Empty,
            TypeScriptDecorationKind.Keyword => "bte_keyword",
            TypeScriptDecorationKind.String => "bte_string-literal",
            TypeScriptDecorationKind.Comment => "bte_comment",
            _ => string.Empty,
        };
    }
}