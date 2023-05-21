using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Analysis.C.Facts;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.RazorLib.Analysis.C.SyntaxActors;

public class TextEditorCLexer : ITextEditorLexer
{
    public static readonly GenericPreprocessorDefinition CPreprocessorDefinition = new(
        "#",
        new []
        {
            new DeliminationExtendedSyntaxDefinition(
                "<",
                ">",
                GenericDecorationKind.DeliminationExtended)
        }.ToImmutableArray());
    
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
        CKeywords.ALL,
        CPreprocessorDefinition);

    private readonly GenericSyntaxTree _cSyntaxTree;

    public TextEditorCLexer()
    {
        _cSyntaxTree = new GenericSyntaxTree(CLanguageDefinition); 
    }

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var cSyntaxUnit = _cSyntaxTree
            .ParseText(text);

        var cSyntaxWalker = new GenericSyntaxWalker();

        cSyntaxWalker.Visit(cSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericStringSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericCommentSingleLineSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericCommentMultiLineSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericFunctionSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericPreprocessorDirectiveSyntaxes
                .Select(x => x.TextEditorTextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.GenericDeliminationExtendedSyntaxes
                .Select(x => x.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}