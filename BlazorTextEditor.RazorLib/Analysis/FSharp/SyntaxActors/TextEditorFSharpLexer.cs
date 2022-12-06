using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;

public class TextEditorFSharpLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var fSharpSyntaxUnit = 
            FSharpSyntaxTree.ParseText(text);

        var fSharpSyntaxWalker = new FSharpSyntaxWalker();

        fSharpSyntaxWalker.Visit(fSharpSyntaxUnit.FSharpDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.FSharpStringSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.FSharpCommentSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.FSharpKeywordSyntaxes
                .Select(x => x.TextEditorTextSpan));
        
        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}