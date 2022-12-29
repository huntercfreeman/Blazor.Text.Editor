using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapVim : TextEditorKeymapDefault
{
    public override TextEditorCommand? Map(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection)
    {
        if (keyboardEventArgs.CtrlKey)
        {
            return DefaultCtrlModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }

        if (keyboardEventArgs.AltKey)
        {
            return DefaultAltModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
            
        if (hasTextSelection)
        {
            return DefaultHasSelectionModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
            
        return keyboardEventArgs.Key switch
        {
            KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandFacts.ScrollPageDown,
            KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandFacts.ScrollPageUp,
            _ => null,
        };
    }
}