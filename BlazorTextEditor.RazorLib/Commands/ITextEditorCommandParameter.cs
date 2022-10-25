using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.MoveThese;

public interface ITextEditorCommandParameter
{
    public TextEditorBase TextEditor { get; }
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; }
    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    public Action ReloadVirtualizationDisplay { get; }
    public Action<TextEditorBase>? OnSaveRequested { get; }
}