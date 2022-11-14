using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record DeleteTextByMotionTextEditorBaseAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    MotionKind MotionKind,
    CancellationToken CancellationToken);