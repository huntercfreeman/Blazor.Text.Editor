using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json;

public class TextEditorJsonLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
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