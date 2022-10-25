using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Keyboard;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public Func<KeyboardEventArgs, TextEditorCommand?> KeymapFunc { get; } = keyboardEventArgs =>
    {
        if (keyboardEventArgs.Key == "c" && keyboardEventArgs.CtrlKey)
        {
            return TextEditorCommandFacts.Copy;
        }
        else if (keyboardEventArgs.Key == "v" && keyboardEventArgs.CtrlKey)
        {
            return TextEditorCommandFacts.Paste;
        }
        else if (keyboardEventArgs.Key == "s" && keyboardEventArgs.CtrlKey)
        {
            return TextEditorCommandFacts.Save;
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE &&
                 keyboardEventArgs.CtrlKey ||
                 keyboardEventArgs.AltKey &&
                 keyboardEventArgs.Key == "a")
        {
            // Short term hack to avoid autocomplete keybind being typed.
        }
        
        return null;
    };
}