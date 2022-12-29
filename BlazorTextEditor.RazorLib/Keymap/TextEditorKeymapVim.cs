using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapVim : ITextEditorKeymap
{
    public KeymapKey KeymapKey => KeymapFacts.VimKeymapDefinition.KeymapKey;
    public string KeymapDisplayName => KeymapFacts.VimKeymapDefinition.DisplayName;

    private readonly TextEditorKeymapDefault _textEditorKeymapDefault = new();
    
    public VimMode ActiveVimMode { get; private set; } = VimMode.Normal;
    
    /*
     Vim Grammar Conditional Branching
     ---------------------------------
     
     []Verb
        []Modifier
            []TextObject
        []TextObject
        []Repeat
            []TextObject
    []TextObject
    []Repeat
        []TextObject 
     */

    private readonly VimSentence _vimSentence = new();
    
    public string GetCursorCssClassString()
    {
        return ActiveVimMode switch
        {
            VimMode.Normal => TextCursorKindFacts.BlockCssClassString,
            _ => string.Empty
        };
    }
    
    public string GetCursorCssStyleString(
        TextEditorBase textEditorBase,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        switch (ActiveVimMode)
        {
            case VimMode.Normal:
                return $"width: {characterWidthInPixels}px;";
        }

        return string.Empty;
    }
    
    public TextEditorCommand? Map(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (TryMapToVimKeymap(
                keyboardEventArgs,
                hasTextSelection,
                out var command))
        {
            return command;
        }
        
        if (keyboardEventArgs.CtrlKey)
        {
            return _textEditorKeymapDefault.DefaultCtrlModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
        
        if (keyboardEventArgs.CtrlKey)
        {
            return _textEditorKeymapDefault.DefaultCtrlModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }

        if (keyboardEventArgs.AltKey)
        {
            return _textEditorKeymapDefault.DefaultAltModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
            
        if (hasTextSelection)
        {
            return _textEditorKeymapDefault.DefaultHasSelectionModifiedKeymap(
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

    public bool TryMapToVimKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            {
                if (TryMapToVimNormalModeKeymap(
                        keyboardEventArgs,
                        hasTextSelection,
                        out command))
                {
                    return true;
                }

                goto default;
            }
            case VimMode.Insert:
            {
                if (TryMapToVimInsertModeKeymap(
                        keyboardEventArgs,
                        hasTextSelection,
                        out command))
                {
                    return true;
                }

                goto default;
            }
            default:
            {
                command = null;
                return false;
            }
        }
    }

    public bool TryMapToVimNormalModeKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        switch (keyboardEventArgs.Key)
        {
            case "i":
            {
                ActiveVimMode = VimMode.Insert;
                command = TextEditorCommandFacts.DoNothingDiscard;
                return true;
            }
            default:
            {
                command = null;
                return false;
            }
        }
    }
    
    public bool TryMapToVimInsertModeKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand command)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MetaKeys.ESCAPE:
            {
                ActiveVimMode = VimMode.Normal;
                command = TextEditorCommandFacts.DoNothingDiscard;
                return true;
            }
            default:
            {
                command = null;
                return false;
            }
        }
    }
}