using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Commands;

public interface ITextEditorCommandParameter
{
    public TextEditorBase TextEditor { get; }
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; }
    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    /// <summary>
    /// TODO: <see cref="TextEditorDisplay.ReloadVirtualizationDisplay"/> is a hack and should be rewritten correctly. The modification of the TextEditorBase should send a notification automatically to all Blazor components that reference it by TextEditorKey. Allowing them an option to choose to re-render.
    /// </summary>
    public TextEditorDisplay TextEditorDisplay { get; }
}