using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Facts;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;

public class TextEditorFSharpLexer : ITextEditorLexer
{
    public static readonly GenericPreprocessorDefinition FSharpPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);
    
    public static readonly GenericLanguageDefinition FSharpLanguageDefinition = new GenericLanguageDefinition(
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
        "(*",
        "*)",
        FSharpKeywords.ALL,
        FSharpPreprocessorDefinition);

    private readonly GenericSyntaxTree _fSharpSyntaxTree;

    public TextEditorFSharpLexer()
    {
        _fSharpSyntaxTree = new GenericSyntaxTree(FSharpLanguageDefinition); 
    }

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var fSharpSyntaxUnit = _fSharpSyntaxTree
            .ParseText(text);

        var fSharpSyntaxWalker = new GenericSyntaxWalker();

        fSharpSyntaxWalker.Visit(fSharpSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.GenericStringSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.GenericCommentSingleLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.GenericCommentMultiLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.GenericKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.GenericFunctionSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}