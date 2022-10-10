using System.Collections.Immutable;

namespace BlazorTextEditor.ClassLib.Lexing;

public interface ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string content);
}