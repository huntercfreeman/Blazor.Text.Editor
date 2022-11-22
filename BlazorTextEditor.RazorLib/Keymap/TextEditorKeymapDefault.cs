using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Keyboard;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public Func<KeyboardEventArgs, TextEditorCommand?> KeymapFunc { get; } =
        keyboardEventArgs =>
        {
            if (keyboardEventArgs.CtrlKey)
                return CtrlModifiedKeymap(keyboardEventArgs);

            if (keyboardEventArgs.AltKey)
                return AltModifiedKeymap(keyboardEventArgs);

            return null;
        };

    private static TextEditorCommand? CtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.AltKey)
            return CtrlAltModifiedKeymap(keyboardEventArgs);

        var command = keyboardEventArgs.Key switch
        {
            "x" => TextEditorCommandFacts.Cut,
            "c" => TextEditorCommandFacts.Copy,
            "v" => TextEditorCommandFacts.Paste,
            "s" => TextEditorCommandFacts.Save,
            "a" => TextEditorCommandFacts.SelectAll,
            "z" => TextEditorCommandFacts.Undo,
            "y" => TextEditorCommandFacts.Redo,
            KeyboardKeyFacts.MovementKeys.ARROW_DOWN => TextEditorCommandFacts.ScrollLineDown,
            _ => null,
        };

        if (command is null)
        {
            command = keyboardEventArgs.Code switch
            {
                KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE => 
                    TextEditorCommandFacts.DoNothingDiscard,
                _ => null,
            };
        }

        return command;
    }

    /// <summary>
    ///     Do not go from <see cref="AltModifiedKeymap" /> to
    ///     <see cref="CtrlAltModifiedKeymap" />
    ///     <br /><br />
    ///     Code in this method should only be here if it
    ///     does not include a Ctrl key being pressed.
    ///     <br /><br />
    ///     As otherwise, we'd have to permute over
    ///     all the possible keyboard modifier
    ///     keys and have a method for each permutation.
    /// </summary>
    private static TextEditorCommand? AltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "a")
        {
            // Short term hack to avoid autocomplete keybind being typed.
        }

        return null;
    }

    private static TextEditorCommand? CtrlAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        return null;
    }
}