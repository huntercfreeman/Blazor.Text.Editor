using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Commands;

public class TextEditorCommandParameter : ITextEditorCommandParameter
{
    public TextEditorCommandParameter(
        TextEditorBase textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        IClipboardProvider clipboardProvider,
        ITextEditorService textEditorService,
        Action reloadVirtualizationDisplay,
        Action<TextEditorBase>? onSaveRequested)
    {
        TextEditor = textEditor;
        CursorSnapshots = cursorSnapshots;
        ClipboardProvider = clipboardProvider;
        TextEditorService = textEditorService;
        ReloadVirtualizationDisplay = reloadVirtualizationDisplay;
        OnSaveRequested = onSaveRequested;
    }

    public TextEditorBase TextEditor { get; }

    public TextEditorCursorSnapshot PrimaryCursorSnapshot => CursorSnapshots
        .First(x => x.UserCursor.IsPrimaryCursor);

    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    /// <summary>
    /// TODO: <see cref="ReloadVirtualizationDisplay"/> is a hack and should be rewritten correctly. The modification of the TextEditorBase should send a notification automatically to all Blazor components that reference it by TextEditorKey. Allowing them an option to choose to re-render.
    /// </summary>
    public Action ReloadVirtualizationDisplay { get; }
    public Action<TextEditorBase>? OnSaveRequested { get; }
}