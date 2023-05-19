using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Semantics;

public interface ISemanticModel
{
    public ImmutableList<TextEditorTextSpan> TextEditorTextSpans { get; }

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan);
    
    public void Parse(
        TextEditorModel model);
}