using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;

public class TextEditorTypeScriptLexer : ITextEditorLexer
{
    public static readonly GenericPreprocessorDefinition TypeScriptPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);
    
    public static readonly GenericLanguageDefinition TypeScriptLanguageDefinition = new GenericLanguageDefinition(
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
        TypeScriptKeywords.ALL,
        TypeScriptPreprocessorDefinition);

    private readonly GenericSyntaxTree _typeScriptSyntaxTree;

    public TextEditorTypeScriptLexer()
    {
        _typeScriptSyntaxTree = new GenericSyntaxTree(TypeScriptLanguageDefinition);
    }

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var typeScriptSyntaxUnit = _typeScriptSyntaxTree
            .ParseText(text);

        var typeScriptSyntaxWalker = new GenericSyntaxWalker();

        typeScriptSyntaxWalker.Visit(typeScriptSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(typeScriptSyntaxWalker.GenericStringSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(typeScriptSyntaxWalker.GenericCommentSingleLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(typeScriptSyntaxWalker.GenericCommentMultiLineSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(typeScriptSyntaxWalker.GenericKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(typeScriptSyntaxWalker.GenericFunctionSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}