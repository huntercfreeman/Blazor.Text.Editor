using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.RazorLib.Semantics;

public interface ISemanticModel
{
    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan);
    
    /// <summary>
    /// Preferably the SemanticModel will update automatically.
    /// Early in development though a lot of events are not set-up
    /// and so this is a helpful method.
    /// </summary>
    public void ManuallyRefreshSemanticModel(
        TextEditorModel model);
}