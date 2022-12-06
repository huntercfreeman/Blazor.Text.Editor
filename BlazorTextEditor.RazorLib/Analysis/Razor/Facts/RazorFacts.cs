using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.Facts;

public static class RazorFacts
{
    public const string TRANSITION_SUBSTRING = "@";
    public const string TRANSITION_SUBSTRING_ESCAPED = "@@";
    
    /// <summary>
    /// Only valid if follows immediately after <see cref="TRANSITION_SUBSTRING"/>
    /// </summary>
    public const char COMMENT_START = '*';
    
    /// <summary>
    /// Only valid if is immediately before <see cref="TRANSITION_SUBSTRING"/>
    /// </summary>
    public const char COMMENT_END = '*';
}