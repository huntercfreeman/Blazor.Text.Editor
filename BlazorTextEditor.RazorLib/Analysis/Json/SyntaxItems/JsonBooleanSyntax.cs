﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

public class JsonBooleanSyntax : IJsonSyntax
{
    public JsonBooleanSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => ImmutableArray<IJsonSyntax>.Empty;

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Boolean;
}