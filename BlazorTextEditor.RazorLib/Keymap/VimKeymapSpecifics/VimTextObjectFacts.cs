using System.Collections.Immutable;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimTextObjectFacts
{
    public static bool TryConstructTextObjectToken(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        switch (keyboardEventArgs.Key)
        {
            case "w":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.TextObject,
                    keyboardEventArgs.Key);

                return true;
            }
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParseVimSentence(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        var testingThings = new TextEditorCommand(
            parameter =>
            {
                var word = new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    CtrlKey = true
                };
                
                TextEditorCursor.MoveCursor(
                    word,
                    parameter.PrimaryCursorSnapshot.UserCursor,
                    parameter.TextEditorBase);
                
                return Task.CompletedTask;
            },
            true,
            "Word",
            "word",
            TextEditKind.None);
        
        switch (keyboardEventArgs.Key)
        {
            case "w":
            {
                textEditorCommand = testingThings;
                return true;
            }
        }
        
        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }
}