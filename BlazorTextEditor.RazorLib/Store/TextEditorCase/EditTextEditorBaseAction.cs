using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase;

public record EditTextEditorBaseAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    KeyboardEventArgs KeyboardEventArgs,
    CancellationToken CancellationToken);