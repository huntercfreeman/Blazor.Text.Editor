using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class TextEditorJavaScriptLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var textEditorTextSpans = 
            JavaScriptSyntaxTree.ParseText(text);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}