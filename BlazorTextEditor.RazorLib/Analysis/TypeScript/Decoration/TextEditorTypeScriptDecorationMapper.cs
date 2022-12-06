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
            _ => string.Empty,
        };
    }
}
