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
            jsonSyntaxWalker.JsonLineCommentSyntaxes.Select(lineComment =>
                lineComment.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}