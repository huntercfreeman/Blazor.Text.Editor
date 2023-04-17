using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Facts;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;

public class TextEditorCSharpLexer : ILexer
{
    public static readonly GenericPreprocessorDefinition CSharpPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntax>.Empty);
    
    public static readonly GenericLanguageDefinition CSharpLanguageDefinition = new GenericLanguageDefinition(
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
        CSharpKeywords.ALL,
        CSharpPreprocessorDefinition);

    private readonly GenericSyntaxTree _cSharpSyntaxTree;

    public TextEditorCSharpLexer()
    {
        _cSharpSyntaxTree = new GenericSyntaxTree(CSharpLanguageDefinition); 
    }
    
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var cSharpSyntaxUnit = _cSharpSyntaxTree
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