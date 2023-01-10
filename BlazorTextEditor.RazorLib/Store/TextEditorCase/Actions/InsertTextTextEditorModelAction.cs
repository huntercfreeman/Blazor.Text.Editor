using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record InsertTextTextEditorModelAction(
    TextEditorModelKey TextEditorModelKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    string Content,
    CancellationToken CancellationToken);