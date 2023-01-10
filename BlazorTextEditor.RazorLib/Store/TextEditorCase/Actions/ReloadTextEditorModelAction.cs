using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record ReloadTextEditorModelAction(TextEditorModelKey TextEditorModelKey, string Content);