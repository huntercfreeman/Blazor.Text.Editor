using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

/// <summary>
/// TODO: I presume that the <see cref="TextEditorJavaScriptLexer"/>
/// and the <see cref="TextEditorTypeScriptLexer"/> can share logic.
/// </summary>
public class TextEditorFSharpLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var textEditorTextSpans = 
            TypeScriptSyntaxTree.ParseText(text);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}