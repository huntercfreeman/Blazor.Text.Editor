using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record DeleteTextByRangeTextEditorBaseAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    int Count,
    CancellationToken CancellationToken);