using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;

namespace BlazorTextEditor.RazorLib.TextEditor;

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
    public Action ReloadVirtualizationDisplay { get; }
    public Action<TextEditorBase>? OnSaveRequested { get; }
}