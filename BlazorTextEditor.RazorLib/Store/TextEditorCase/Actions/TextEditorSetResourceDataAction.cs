using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record TextEditorSetResourceDataAction(
    TextEditorKey TextEditorKey,
    string ResourceUri,
    DateTime ResourceLastWriteTime);