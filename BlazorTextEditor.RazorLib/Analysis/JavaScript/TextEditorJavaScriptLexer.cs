using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class TextEditorJavaScriptLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string content)
    {
        var textEditorTextSpans = 
            JavaScriptSyntaxTree.ParseText(content);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}