using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Commands.Vim;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.Vim;

public static class SyntaxVerbVim
{
    public static bool TryLex(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        if (keyboardEventArgs.CtrlKey)
        {
            switch (keyboardEventArgs.Key)
            {
                case "e":
                {
                    vimGrammarToken = new VimGrammarToken(
                        VimGrammarKind.Verb,
                        keyboardEventArgs);

                    return true;
                }
                case "y":
                {
                    vimGrammarToken = new VimGrammarToken(
                        VimGrammarKind.Verb,
                        keyboardEventArgs);

                    return true;
                }
            }
        }

        switch (keyboardEventArgs.Key)
        {
            case "d":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.Verb,
                    keyboardEventArgs);

                return true;
            }
            case "c":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.Verb,
                    keyboardEventArgs);

                return true;
            }
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        bool verbWasTypedTwoTimesInARow = false;

        var currentToken = sentenceSnapshot[indexInSentence];

        if (indexInSentence + 1 < sentenceSnapshot.Length)
        {
            var nextToken = sentenceSnapshot[indexInSentence + 1];

            if (nextToken.VimGrammarKind == VimGrammarKind.Verb &&
                nextToken.KeyboardEventArgs.Key == currentToken.KeyboardEventArgs.Key)
            {
                verbWasTypedTwoTimesInARow = true;
            }
        }

        if (verbWasTypedTwoTimesInARow)
        {
            // TODO: When a verb is doubled is it always the case that the position indices to operate over are known without the need of a motion? Example, "dd" would delete the current line and copy it to the in memory clipboard. But no motion was needed to know what text to delete.
            switch (currentToken.KeyboardEventArgs.Key)
            {
                case "d":
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.DeleteLine;
                    return true;
                case "c":
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeLine;
                    return true;
            }
        }

        if (keyboardEventArgs.CtrlKey)
        {
            switch (currentToken.KeyboardEventArgs.Key)
            {
                case "e":
                    textEditorCommand = TextEditorCommandDefaultFacts.ScrollLineDown;
                    return true;
                case "y":
                    textEditorCommand = TextEditorCommandDefaultFacts.ScrollLineUp;
                    return true;
            }
        }
        else
        {
            // Track locally the displacement of the user's cursor after the
            // inner text editor command is invoked.

            if (VimSentence.TryParseNextToken(
                    textEditorKeymapVim,
                    sentenceSnapshot,
                    indexInSentence + 1,
                    keyboardEventArgs,
                    hasTextSelection,
                    out var innerTextEditorCommand) &&
                innerTextEditorCommand is not null)
            {
                switch (currentToken.KeyboardEventArgs.Key)
                {
                    case "d":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.GetDeleteMotion(
                            innerTextEditorCommand);

                        return true;
                    case "c":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.GetChangeMotion(
                            innerTextEditorCommand);

                        return true;
                }
            }
        }

        textEditorCommand = null;
        return false;
    }
}