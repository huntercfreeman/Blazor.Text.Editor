using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
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
        switch (keyboardEventArgs.Key)
        {
            case "w":
            {
                textEditorCommand = TextEditorCommandFacts.SelectAll;
                return true;
            }
        }
        
        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }
}