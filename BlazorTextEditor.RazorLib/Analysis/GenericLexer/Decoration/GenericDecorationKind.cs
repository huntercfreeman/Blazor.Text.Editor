namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;

public enum GenericDecorationKind
{
    None,
    Keyword,
    CommentSingleLine,
    CommentMultiLine,
    Error,
    StringLiteral,
    Function,
    PreprocessorDirective
}