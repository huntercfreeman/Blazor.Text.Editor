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
            case "e":
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
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        var currentToken = sentenceSnapshot[indexInSentence];
        
        switch (currentToken.TextValue)
        {
            case "w":
            {
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        VimTextEditorMotionFacts.Word(
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim NextWord",
                    "vim_next-word");
                
                return true;
            }
            case "e":
            {
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        VimTextEditorMotionFacts.End(
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "VimEndWord",
                    "vim_end-word");
                
                return true;
            }
        }
        
        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }
}