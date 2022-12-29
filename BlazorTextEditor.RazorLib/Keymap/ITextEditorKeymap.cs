using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public interface ITextEditorKeymap
{
    public TextEditorCommand? Map(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection);
    public KeymapKey KeymapKey { get; }
    public string KeymapDisplayName { get; }
}