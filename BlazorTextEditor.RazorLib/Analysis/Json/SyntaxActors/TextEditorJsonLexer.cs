using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;

public class TextEditorJsonLexer : ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var jsonSyntaxUnit = JsonSyntaxTree.ParseText(text);
        
        var syntaxNodeRoot = jsonSyntaxUnit.JsonDocumentSyntax;

        var jsonSyntaxWalker = new JsonSyntaxWalker();

        jsonSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonPropertyKeySyntaxes.Select(propertyKey =>
                propertyKey.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonBooleanSyntaxes.Select(boolean =>
                boolean.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonIntegerSyntaxes.Select(integer =>
                integer.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonNullSyntaxes.Select(n =>
                n.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonNumberSyntaxes.Select(number =>
                number.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonStringSyntaxes.Select(s =>
                s.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}