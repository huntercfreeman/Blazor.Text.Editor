using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Semantics;

public interface ISemanticModel
{
    public ImmutableList<TextEditorTextSpan> DiagnosticTextSpans { get; set; }
    public ImmutableList<TextEditorTextSpan> SymbolTextSpans { get; }

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan);
    
    public void Parse(
        TextEditorModel model);
}
