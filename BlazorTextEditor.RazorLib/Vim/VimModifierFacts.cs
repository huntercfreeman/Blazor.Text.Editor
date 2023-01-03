using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Keymap.Vim;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Vim;

public static class VimModifierFacts
{
    public static bool TryConstructModifierToken(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        switch (keyboardEventArgs.Key)
        {
            case "i":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.Modifier,
                    keyboardEventArgs);

                return true;
            }
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParseModifier(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }
}