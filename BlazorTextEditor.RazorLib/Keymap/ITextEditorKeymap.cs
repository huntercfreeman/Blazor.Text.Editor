using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;
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