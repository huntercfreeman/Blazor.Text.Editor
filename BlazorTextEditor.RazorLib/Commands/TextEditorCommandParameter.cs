using System.Collections.Immutable;
using BlazorALaCarte.Shared.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Commands;

public class TextEditorCommandParameter : ITextEditorCommandParameter
{
    public TextEditorCommandParameter(
        TextEditorBase textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        IClipboardProvider clipboardProvider,
        ITextEditorService textEditorService,
        TextEditorViewModel textEditorViewModel)
    {
        TextEditorBase = textEditor;
        CursorSnapshots = cursorSnapshots;
        ClipboardProvider = clipboardProvider;
        TextEditorService = textEditorService;
        TextEditorViewModel = textEditorViewModel;
    }

    public TextEditorBase TextEditorBase { get; }

    public TextEditorCursorSnapshot PrimaryCursorSnapshot => CursorSnapshots
        .First(x => x.UserCursor.IsPrimaryCursor);

    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    public TextEditorViewModel TextEditorViewModel { get; }
}