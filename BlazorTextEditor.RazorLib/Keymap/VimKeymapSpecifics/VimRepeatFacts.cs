using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimRepeatFacts
{
    public static bool TryConstructRepeatToken(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        if (keyboardEventArgs.Key.Length != 1)
        {
            vimGrammarToken = null;
            return false;
        }

        var possibleNumeric = keyboardEventArgs.Key.Single();

        if (char.IsNumber(possibleNumeric))
        {
            vimGrammarToken = new VimGrammarToken(
                VimGrammarKind.Repeat,
                keyboardEventArgs.Key);

            return true;
        }

        vimGrammarToken = null;
        return false;
    }
}