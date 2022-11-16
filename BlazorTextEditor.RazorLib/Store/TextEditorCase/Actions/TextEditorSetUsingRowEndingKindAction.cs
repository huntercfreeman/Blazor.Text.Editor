using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record TextEditorSetUsingRowEndingKindAction(
    TextEditorKey TextEditorKey,
    RowEndingKind RowEndingKind);