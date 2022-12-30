﻿using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimRepeatFacts
{
    public static bool TryConstructRepeatToken(
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

        if (char.IsNumber(possibleNumeric))
        {
            vimGrammarToken = new VimGrammarToken(
                VimGrammarKind.Repeat,
                keyboardEventArgs.Key);

            return true;
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
        int modifiedIndexInSentence = indexInSentence;
        
        var numberBuilder = new StringBuilder();

        for (int i = indexInSentence; i < sentenceSnapshot.Length; i++)
        {
            var currentToken = sentenceSnapshot[i];

            if (currentToken.VimGrammarKind == VimGrammarKind.Repeat)
            {
                numberBuilder.Append(currentToken.TextValue);
                modifiedIndexInSentence++;
            }
        }
        
        var intValue = Int32.Parse(numberBuilder.ToString());

        var success = VimSentence.TryParseMoveNext(
            sentenceSnapshot,
            modifiedIndexInSentence,
            keyboardEventArgs,
            hasTextSelection,
            out var innerTextEditorCommand);

        var textEditorCommandDisplayName = 
            $"do{intValue}Times: {innerTextEditorCommand.DisplayName}";
        
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