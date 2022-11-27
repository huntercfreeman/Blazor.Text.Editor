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
            CssDecorationKind.TagSelector => "bte_tag-selector",
            CssDecorationKind.PropertyName => "bte_css-property-name",
            CssDecorationKind.PropertyValue => "bte_css-property-value",
            CssDecorationKind.Comment => "bte_comment",
            _ => string.Empty,
        };
    }
}
