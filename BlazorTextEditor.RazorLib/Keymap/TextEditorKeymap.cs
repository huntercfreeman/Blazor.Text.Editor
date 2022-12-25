using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymap : ITextEditorKeymap
{
    public TextEditorKeymap(
        Func<(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection), TextEditorCommand> keymapFunc)
    {
        KeymapFunc = keymapFunc;
    }

    public Func<(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection), TextEditorCommand> KeymapFunc { get; }
}