using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.Default;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public KeymapKey KeymapKey => KeymapFacts.DefaultKeymapDefinition.KeymapKey;
    public string KeymapDisplayName => KeymapFacts.DefaultKeymapDefinition.DisplayName;

    public string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BeamCssClassString;
    }
    
    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        return string.Empty;
    }
    
    public TextEditorCommand? Map(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection)
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

        if (keyboardEventArgs.ShiftKey &&
            KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code)
        {
            return TextEditorCommandDefaultFacts.NewLineBelow;
        }
            
        return keyboardEventArgs.Key switch
        {
            KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandDefaultFacts.ScrollPageDown,
            KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandDefaultFacts.ScrollPageUp,
            _ => null,
        };
    }

    public TextEditorCommand? DefaultCtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.AltKey)
            return DefaultCtrlAltModifiedKeymap(keyboardEventArgs, hasTextSelection);

        var command = keyboardEventArgs.Key switch
        {
            "x" => TextEditorCommandDefaultFacts.Cut,
            "c" => TextEditorCommandDefaultFacts.Copy,
            "v" => TextEditorCommandDefaultFacts.Paste,
            "s" => TextEditorCommandDefaultFacts.Save,
            "a" => TextEditorCommandDefaultFacts.SelectAll,
            "z" => TextEditorCommandDefaultFacts.Undo,
            "y" => TextEditorCommandDefaultFacts.Redo,
            "d" => TextEditorCommandDefaultFacts.Duplicate,
            "]" => TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false),
            "}" => TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true),
            KeyboardKeyFacts.MovementKeys.ARROW_DOWN => TextEditorCommandDefaultFacts.ScrollLineDown,
            KeyboardKeyFacts.MovementKeys.ARROW_UP => TextEditorCommandDefaultFacts.ScrollLineUp,
            KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandDefaultFacts.CursorMovePageBottom,
            KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandDefaultFacts.CursorMovePageTop,
            _ => null,
        };

        if (command is null)
        {
            command = keyboardEventArgs.Code switch
            {
                KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE => TextEditorCommandDefaultFacts.NewLineAbove,
                _ => null,
            };
        }

        return command;
    }

    /// <summary>
    ///     Do not go from <see cref="DefaultAltModifiedKeymap" /> to
    ///     <see cref="DefaultCtrlAltModifiedKeymap" />
    ///     <br /><br />
    ///     Code in this method should only be here if it
    ///     does not include a Ctrl key being pressed.
    ///     <br /><br />
    ///     As otherwise, we'd have to permute over
    ///     all the possible keyboard modifier
    ///     keys and have a method for each permutation.
    /// </summary>
    public TextEditorCommand? DefaultAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
    
    public TextEditorCommand? DefaultHasSelectionModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            if (keyboardEventArgs.ShiftKey)
                return TextEditorCommandDefaultFacts.IndentLess;
            else
                return TextEditorCommandDefaultFacts.IndentMore;
        }
        
        return null;
    }

    public TextEditorCommand? DefaultCtrlAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
}