using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Lexing;

public class LexerDefault : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        return Task.FromResult(ImmutableArray<TextEditorTextSpan>.Empty);
    }
}