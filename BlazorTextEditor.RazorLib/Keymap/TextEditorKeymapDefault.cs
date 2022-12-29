using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapDefault : ITextEditorKeymap
{
    public virtual KeymapKey KeymapKey => KeymapFacts.DefaultKeymapDefinition.KeymapKey;
    public virtual string KeymapDisplayName => KeymapFacts.DefaultKeymapDefinition.DisplayName;

    public virtual string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BeamCssClassString;
    }
    
    public virtual string GetCursorCssStyleString(
        TextEditorBase textEditorBase,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        return string.Empty;
    }
    
    public virtual TextEditorCommand? Map(KeyboardEventArgs keyboardEventArgs, bool hasTextSelection)
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

    protected TextEditorCommand? DefaultCtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.AltKey)
            return DefaultCtrlAltModifiedKeymap(keyboardEventArgs, hasTextSelection);

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
    protected TextEditorCommand? DefaultAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
    
    protected TextEditorCommand? DefaultHasSelectionModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            if (keyboardEventArgs.ShiftKey)
                return TextEditorCommandFacts.IndentLess;
            else
                return TextEditorCommandFacts.IndentMore;
        }
        
        return null;
    }

    protected TextEditorCommand? DefaultCtrlAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        return null;
    }
}