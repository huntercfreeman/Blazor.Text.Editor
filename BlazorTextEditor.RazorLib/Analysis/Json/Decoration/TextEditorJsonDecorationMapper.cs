using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.Json.Decoration;

public class TextEditorJsonDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (JsonDecorationKind)decorationByte;

        return decoration switch
        {
            JsonDecorationKind.PropertyKey => "bte_json-property-key",
            JsonDecorationKind.String => "bte_string-literal",
            JsonDecorationKind.Number => "bte_number",
            JsonDecorationKind.Integer => "bte_integer",
            JsonDecorationKind.Keyword => "bte_keyword",
            JsonDecorationKind.LineComment => "bte_comment",
            JsonDecorationKind.BlockComment => "bte_comment",
            JsonDecorationKind.None => string.Empty,
            JsonDecorationKind.Null => string.Empty,
            JsonDecorationKind.Document => string.Empty,
            JsonDecorationKind.Error => string.Empty,
            _ => string.Empty,
        };
    }
}
