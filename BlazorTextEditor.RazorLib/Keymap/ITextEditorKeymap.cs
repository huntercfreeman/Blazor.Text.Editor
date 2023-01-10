using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public interface ITextEditorKeymap
{
    public TextEditorCommand? Map(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection);
    public KeymapKey KeymapKey { get; }
    public string KeymapDisplayName { get; }
    
    public string GetCursorCssClassString();

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions);
}