using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.C.Facts;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.C.SyntaxActors;

public class TextEditorCLexer : ILexer
{
    public static readonly GenericLanguageDefinition CLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "//",
        new []
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        }.ToImmutableArray(),
        "/*",
        "*/",
        CKeywords.ALL);

    private readonly GenericSyntaxTree _cSyntaxTree;

    public TextEditorCLexer()
    {
        _cSyntaxTree = new GenericSyntaxTree(CLanguageDefinition); 
    }
    
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var cSharpSyntaxUnit = _cSyntaxTree
            .ParseText(text);

        var cSharpSyntaxWalker = new GenericSyntaxWalker();

        cSharpSyntaxWalker.Visit(cSharpSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(cSharpSyntaxWalker.GenericStringSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(cSharpSyntaxWalker.GenericCommentSingleLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(cSharpSyntaxWalker.GenericCommentMultiLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(cSharpSyntaxWalker.GenericKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(cSharpSyntaxWalker.GenericFunctionSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}