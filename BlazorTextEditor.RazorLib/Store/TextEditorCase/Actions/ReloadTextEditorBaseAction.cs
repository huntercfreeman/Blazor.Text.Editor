using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record ReloadTextEditorBaseAction(TextEditorKey TextEditorKey, string Content);