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

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.JavaScriptStringSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.JavaScriptCommentSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.JavaScriptKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}