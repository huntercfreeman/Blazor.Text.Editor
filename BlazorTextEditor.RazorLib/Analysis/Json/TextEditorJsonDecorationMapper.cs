using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.Json;

public class TextEditorJsonDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (JsonDecorationKind)decorationByte;

        return decoration switch
        {
            JsonDecorationKind.None => string.Empty,
            JsonDecorationKind.PropertyKey => "bte_json-property-key",
            JsonDecorationKind.PropertyValue => "bte_json-property-value",
            JsonDecorationKind.String => "bte_string-literal",
            JsonDecorationKind.Keyword => "bte_keyword",
            JsonDecorationKind.LineComment => "bte_comment",
            _ => string.Empty,
        };
    }
}
