using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record ReduceReloadTextEditorBaseAction(TextEditorKey TextEditorKey, string Content);