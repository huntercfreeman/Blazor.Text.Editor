using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public class TextEditorCssLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var textEditorTextSpans = 
            CssSyntaxTree.ParseText(text);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}