using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer;

public class GenericLanguageDefinition
{
    public GenericLanguageDefinition(
        string stringStart,
        string stringEnd,
        string commentSingleLineStart,
        ImmutableArray<string> commentSingleLineEndings,
        string commentMultiLineStart,
        string commentMultiLineEnd,
        ImmutableArray<string> keywords)
    {
        StringStart = stringStart;
        StringEnd = stringEnd;
        CommentSingleLineStart = commentSingleLineStart;
        CommentSingleLineEndings = commentSingleLineEndings;
        CommentMultiLineStart = commentMultiLineStart;
        CommentMultiLineEnd = commentMultiLineEnd;
        Keywords = keywords;
    }
    
    public string StringStart { get; }
    public string StringEnd { get; }
    public string CommentSingleLineStart { get; }
    public ImmutableArray<string> CommentSingleLineEndings { get; }
    public string CommentMultiLineStart { get; }
    public string CommentMultiLineEnd { get; }
    public ImmutableArray<string> Keywords { get; }
}
