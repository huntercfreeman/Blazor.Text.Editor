namespace BlazorTextEditor.RazorLib.Analysis.Json;

public enum JsonSyntaxKind
{
    Unknown,
    PropertyKey,
    PropertyValue,
    String,
    Keyword,
    LineComment,
    BlockComment,
    Document,
    Object,
    Property,
    Array,
    Number,
    Integer,
    Boolean,
    Null
}