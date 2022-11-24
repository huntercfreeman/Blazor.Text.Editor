using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

public class TextEditorFSharpLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var textEditorTextSpans = 
            TypeScriptSyntaxTree.ParseText(text);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}