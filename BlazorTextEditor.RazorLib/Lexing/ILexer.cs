using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Lexing;

public interface ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string content);
}