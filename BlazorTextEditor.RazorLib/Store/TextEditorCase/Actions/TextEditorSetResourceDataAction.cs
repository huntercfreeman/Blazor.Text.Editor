using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record TextEditorSetResourceDataAction(
    TextEditorModelKey TextEditorModelKey,
    string ResourceUri,
    DateTime ResourceLastWriteTime);