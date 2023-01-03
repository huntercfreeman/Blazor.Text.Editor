using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.Vim;

public static class SyntaxRepeatVim
{
    public static bool TryLex(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        if (keyboardEventArgs.Key.Length != 1)
        {
            vimGrammarToken = null;
            return false;
        }

        var possibleNumeric = keyboardEventArgs.Key.Single();

        if (char.IsNumber(possibleNumeric) &&
            possibleNumeric != '0')
        {
            vimGrammarToken = new VimGrammarToken(
                VimGrammarKind.Repeat,
                keyboardEventArgs);

            return true;
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        int modifiedIndexInSentence = indexInSentence;
        
        var numberBuilder = new StringBuilder();

        for (int i = indexInSentence; i < sentenceSnapshot.Length; i++)
        {
            var currentToken = sentenceSnapshot[i];

            if (currentToken.VimGrammarKind == VimGrammarKind.Repeat)
            {
                numberBuilder.Append(currentToken.KeyboardEventArgs.Key);
                modifiedIndexInSentence++;
            }
        }
        
        var intValue = Int32.Parse(numberBuilder.ToString());

        var success = VimSentence.TryParseNextToken(
            textEditorKeymapVim,
            sentenceSnapshot,
            modifiedIndexInSentence,
            keyboardEventArgs,
            hasTextSelection,
            out var innerTextEditorCommand);

        var textEditorCommandDisplayName = 
            $"Vim::Repeat(count: {intValue}," +
            $" arg: {innerTextEditorCommand.DisplayName})";
        
        // Repeat the inner TextEditorCommand using a for loop
        textEditorCommand = new TextEditorCommand(
            async textEditorCommandParameter =>
            {
                for (int index = 0; index < intValue; index++)
                {
                    await innerTextEditorCommand.DoAsyncFunc
                        .Invoke(textEditorCommandParameter);
                }
            },
            true,
            textEditorCommandDisplayName,
            textEditorCommandDisplayName);
        
        
        return success;
    }
}