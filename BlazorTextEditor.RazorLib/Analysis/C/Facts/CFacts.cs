using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.C.Facts;

public static class CFacts
{
    public const char STRING_STARTING_CHARACTER = '"';
    public const char STRING_ENDING_CHARACTER = '"';
    
    public const string COMMENT_SINGLE_LINE_START = "//";
    public const string COMMENT_MULTI_LINE_START = "(*";
    
    public static readonly ImmutableArray<char> COMMENT_SINGLE_LINE_ENDINGS = new []
    {
        WhitespaceFacts.CARRIAGE_RETURN,
        WhitespaceFacts.LINE_FEED,
    }.ToImmutableArray();
    
    public const string COMMENT_MULTI_LINE_END = "*)";
}