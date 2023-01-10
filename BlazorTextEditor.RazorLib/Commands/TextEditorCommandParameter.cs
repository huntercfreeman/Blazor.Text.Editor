using System.Collections.Immutable;
using BlazorALaCarte.Shared.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Commands;

public class TextEditorCommandParameter : ITextEditorCommandParameter
{
    public TextEditorCommandParameter(
        TextEditorModel textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        IClipboardProvider clipboardProvider,
        ITextEditorService textEditorService,
        TextEditorViewModel textEditorViewModel)
    {
        TextEditorModel = textEditor;
        CursorSnapshots = cursorSnapshots;
        ClipboardProvider = clipboardProvider;
        TextEditorService = textEditorService;
        TextEditorViewModel = textEditorViewModel;
    }

    public TextEditorModel TextEditorModel { get; }

    public TextEditorCursorSnapshot PrimaryCursorSnapshot => CursorSnapshots
        .First(x => x.UserCursor.IsPrimaryCursor);

    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    public TextEditorViewModel TextEditorViewModel { get; }
}