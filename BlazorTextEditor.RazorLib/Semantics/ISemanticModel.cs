using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.RazorLib.Semantics;

public interface ISemanticModel
{
    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan);
}