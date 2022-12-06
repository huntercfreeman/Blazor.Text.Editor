namespace BlazorTextEditor.RazorLib.Analysis.Json.Decoration;

public enum JsonDecorationKind
{
    None,
    PropertyKey,
    String,
    Keyword,
    LineComment,
    BlockComment,
    Document,
    Error,
    Null
}