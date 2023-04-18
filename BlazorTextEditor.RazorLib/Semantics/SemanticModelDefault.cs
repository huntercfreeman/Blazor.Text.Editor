using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.RazorLib.Semantics;

public class SemanticModelDefault : ISemanticModel
{
    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        return null;
    }
}