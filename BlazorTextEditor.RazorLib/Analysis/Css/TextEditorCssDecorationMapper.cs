using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public class TextEditorCssDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (CssDecorationKind)decorationByte;

        return decoration switch
        {
            CssDecorationKind.None => string.Empty,
            CssDecorationKind.Keyword => "bte_keyword",
            _ => string.Empty,
        };
    }
}
