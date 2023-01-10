using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record DeleteTextByMotionTextEditorModelAction(
    TextEditorModelKey TextEditorModelKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    MotionKind MotionKind,
    CancellationToken CancellationToken);