using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

public record KeyboardEventTextEditorModelAction(
    TextEditorModelKey TextEditorModelKey,
    ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
    KeyboardEventArgs KeyboardEventArgs,
    CancellationToken CancellationToken);