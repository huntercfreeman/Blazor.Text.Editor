namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;

public enum GenericDecorationKind
{
    None,
    Keyword,
    KeywordControl,
    CommentSingleLine,
    CommentMultiLine,
    Error,
    StringLiteral,
    Function,
    PreprocessorDirective,
    DeliminationExtended
}