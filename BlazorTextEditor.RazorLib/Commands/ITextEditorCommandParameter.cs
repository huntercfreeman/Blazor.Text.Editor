using System.Collections.Immutable;
using BlazorALaCarte.Shared.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Commands;

public interface ITextEditorCommandParameter
{
    public TextEditorModel TextEditorModel { get; }
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; }
    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots { get; }
    public IClipboardProvider ClipboardProvider { get; }
    public ITextEditorService TextEditorService { get; }
    public TextEditorViewModel TextEditorViewModel { get; }
}