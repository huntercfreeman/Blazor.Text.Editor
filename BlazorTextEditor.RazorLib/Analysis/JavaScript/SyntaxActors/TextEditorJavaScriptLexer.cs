using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;

public class TextEditorJavaScriptLexer : ILexer
{
    private readonly ImmutableArray<string> _keywords;

    public TextEditorJavaScriptLexer(ImmutableArray<string> keywords)
    {
        _keywords = keywords;
    }
    
    public TextEditorJavaScriptLexer()
        : this(JavaScriptKeywords.ALL)
    {
    }
    
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var javaScriptSyntaxUnit = 
            JavaScriptSyntaxTree.ParseText(
                text, 
                _keywords);

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