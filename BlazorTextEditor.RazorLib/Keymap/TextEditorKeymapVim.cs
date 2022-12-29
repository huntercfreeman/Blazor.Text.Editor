using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapVim : TextEditorKeymapDefault
{
    public override string KeymapDisplayName => "Vim";

    public VimMode ActiveVimMode { get; private set; } = VimMode.Normal;

    public override string GetCursorCssClassString()
    {
        return ActiveVimMode switch
        {
            VimMode.Normal => TextCursorKindFacts.BlockCssClassString,
            _ => string.Empty
        };
    }
    
    public override string GetCursorCssStyleString(
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