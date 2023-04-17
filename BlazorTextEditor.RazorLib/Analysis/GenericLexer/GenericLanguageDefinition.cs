using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer;

public class GenericLanguageDefinition
{
    public GenericLanguageDefinition(
        string stringStart,
        string stringEnd,
        string functionInvocationStart,
        string functionInvocationEnd,
        string memberAccessToken,
        string commentSingleLineStart,
        ImmutableArray<string> commentSingleLineEndings,
        string commentMultiLineStart,
        string commentMultiLineEnd,
        ImmutableArray<string> keywords,
        GenericPreprocessorDefinition preprocessorDefinition)
    {
        StringStart = stringStart;
        StringEnd = stringEnd;
        FunctionInvocationStart = functionInvocationStart;
        FunctionInvocationEnd = functionInvocationEnd;
        MemberAccessToken = memberAccessToken;
        CommentSingleLineStart = commentSingleLineStart;
        CommentSingleLineEndings = commentSingleLineEndings;
        CommentMultiLineStart = commentMultiLineStart;
        CommentMultiLineEnd = commentMultiLineEnd;
        Keywords = keywords;
        PreprocessorDefinition = preprocessorDefinition;
    }
    
    public string StringStart { get; }
    public string StringEnd { get; }
    public string FunctionInvocationStart { get; }
    public string FunctionInvocationEnd { get; }
    public string MemberAccessToken { get; }
    public string CommentSingleLineStart { get; }
    public ImmutableArray<string> CommentSingleLineEndings { get; }
    public string CommentMultiLineStart { get; }
    public string CommentMultiLineEnd { get; }
    public ImmutableArray<string> Keywords { get; }
    public GenericPreprocessorDefinition PreprocessorDefinition { get; }
}
