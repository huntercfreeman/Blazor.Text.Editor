﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;

public class TextEditorFSharpLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var textEditorTextSpans = 
            FSharpSyntaxTree.ParseText(text);

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}