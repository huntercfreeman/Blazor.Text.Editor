using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json;

public class TextEditorJsonLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var cssSyntaxUnit = JsonSyntaxTree.ParseText(text);
        
        var syntaxNodeRoot = cssSyntaxUnit.JsonDocumentSyntax;

        var cssSyntaxWalker = new JsonSyntaxWalker();

        cssSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}