using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace ExampleApplication.SyntaxHighlighting.FictitiousLanguage;

public class TextEditorFictitiousLanguageLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string content)
    {
        return Task.FromResult(ImmutableArray<TextEditorTextSpan>.Empty);
    }
}