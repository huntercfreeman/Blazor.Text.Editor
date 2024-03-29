﻿using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Semantics;

public class SemanticModelDefault : ISemanticModel
{
    public ImmutableList<TextEditorTextSpan> DiagnosticTextSpans { get; set; } = ImmutableList<TextEditorTextSpan>.Empty;
    public ImmutableList<TextEditorTextSpan> SymbolTextSpans { get; } = ImmutableList<TextEditorTextSpan>.Empty;

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        return null;
    }

    public void Parse(
        TextEditorModel model)
    {
    }
}