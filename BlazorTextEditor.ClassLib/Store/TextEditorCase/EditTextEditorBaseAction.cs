using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

public record EditTextEditorBaseAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<(ImmutableTextEditorCursor immutableTextEditorCursor, TextEditorCursor textEditorCursor)> TextCursorTuples,
    KeyboardEventArgs KeyboardEventArgs,
    CancellationToken CancellationToken);