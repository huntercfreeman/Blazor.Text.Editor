using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record DeleteTextByRangeTextEditorModelAction(
    TextEditorModelKey TextEditorModelKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    int Count,
    CancellationToken CancellationToken);