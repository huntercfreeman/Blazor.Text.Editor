using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public Func<(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection), TextEditorCommand?> KeymapFunc { get; } =
        keyboardEventAndHasSelectionTuple =>
        {
            if (keyboardEventAndHasSelectionTuple.keyboardEventArgs.CtrlKey)
                return CtrlModifiedKeymap(
                    keyboardEventAndHasSelectionTuple.keyboardEventArgs,
                    keyboardEventAndHasSelectionTuple.hasTextSelection);

            if (keyboardEventAndHasSelectionTuple.keyboardEventArgs.AltKey)
                return AltModifiedKeymap(
                    keyboardEventAndHasSelectionTuple.keyboardEventArgs,
                    keyboardEventAndHasSelectionTuple.hasTextSelection);
            
            if (keyboardEventAndHasSelectionTuple.hasTextSelection)
                return HasSelectionModifiedKeymap(
                    keyboardEventAndHasSelectionTuple.keyboardEventArgs,
                    keyboardEventAndHasSelectionTuple.hasTextSelection);
            
            return keyboardEventAndHasSelectionTuple.keyboardEventArgs.Key switch
            {
                KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandFacts.ScrollPageDown,
                KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandFacts.ScrollPageUp,
                _ => null,
            };
        };

    private static TextEditorCommand? CtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.AltKey)
            return CtrlAltModifiedKeymap(keyboardEventArgs, hasTextSelection);

        var command = keyboardEventArgs.Key switch
        {
            "x" => TextEditorCommandFacts.Cut,
            "c" => TextEditorCommandFacts.Copy,
            "v" => TextEditorCommandFacts.Paste,
            "s" => TextEditorCommandFacts.Save,
            "a" => TextEditorCommandFacts.SelectAll,
            "z" => TextEditorCommandFacts.Undo,
            "y" => TextEditorCommandFacts.Redo,
            "d" => TextEditorCommandFacts.Duplicate,
            KeyboardKeyFacts.MovementKeys.ARROW_DOWN => TextEditorCommandFacts.ScrollLineDown,
            KeyboardKeyFacts.MovementKeys.ARROW_UP => TextEditorCommandFacts.ScrollLineUp,
            KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandFacts.CursorMovePageBottom,
            KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandFacts.CursorMovePageTop,
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
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
    
    private static TextEditorCommand? HasSelectionModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            return TextEditorCommandFacts.IndentMore;
        }
        
        return null;
    }

    private static TextEditorCommand? CtrlAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
}