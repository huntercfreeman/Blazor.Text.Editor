using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class TextEditorJavaScriptLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var javaScriptSyntaxUnit = 
            JavaScriptSyntaxTree.ParseText(text);

        var javaScriptSyntaxWalker = new JavaScriptSyntaxWalker();

        javaScriptSyntaxWalker.Visit(javaScriptSyntaxUnit.JavaScriptDocumentSyntax);

        return Task.FromResult(javaScriptSyntaxWalker.JavaScriptStringSyntaxes
            .Select(x => x.TextEditorTextSpan)
            .ToImmutableArray());
    }
}