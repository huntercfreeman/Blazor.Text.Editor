using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record UndoEditAction(TextEditorModelKey TextEditorModelKey);