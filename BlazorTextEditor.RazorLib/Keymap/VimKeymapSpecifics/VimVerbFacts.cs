using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimVerbFacts
{
    public static bool TryConstructVerbToken(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        switch (keyboardEventArgs.Key)
        {
            case "d":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.Verb,
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
        bool verbHasRepeat = false;
        
        var currentToken = sentenceSnapshot[indexInSentence];

        if (indexInSentence + 1 < sentenceSnapshot.Length)
        {
            var nextToken = sentenceSnapshot[indexInSentence + 1];

            if (nextToken.VimGrammarKind == VimGrammarKind.Verb &&
                nextToken.TextValue == currentToken.TextValue)
            {
                verbHasRepeat = true;
            }
        }

        var success = false;

        if (verbHasRepeat)
        {
            // TODO: When a verb is repeated is it always the case that the position indices to operate over are known without the need of a motion? Example, "dd" would delete the current line and copy it to the in memory clipboard. But no motion was needed to know what text to delete.

            success = true;
            
            switch (currentToken.TextValue)
            {
                case "d":
                {
                    // Delete the current line
                    
                    textEditorCommand = new TextEditorCommand(
                        async textEditorCommandParameter =>
                        {                         
                            await TextEditorCommandFacts.Cut.DoAsyncFunc
                                .Invoke(textEditorCommandParameter);
                        },
                        true,
                        "Vim::Delete(Line)",
                        "Vim::Delete(Line)");
                    
                    break;
                }
                default:
                {
                    textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
                    break;
                }
            }
        }
        else
        {
            // Track locally the starting PositionIndex
            //
            // Perform any Vim Motion to displace the cursor
            //
            // Delete inclusively the starting PositionIndex up to
            // the exclusive ending PositionIndex.
            
            
            success = VimSentence.TryParseMoveNext(
                sentenceSnapshot,
                indexInSentence + 1,
                keyboardEventArgs,
                hasTextSelection,
                out var innerTextEditorCommand);

            var displayName = 
                $"Vim::Delete({innerTextEditorCommand.DisplayName})";

            textEditorCommand = new TextEditorCommand(
                async textEditorCommandParameter =>
                {
                    var currentPrimaryImmutableCursor = textEditorCommandParameter
                        .PrimaryCursorSnapshot.ImmutableCursor;
                    
                    await innerTextEditorCommand.DoAsyncFunc
                        .Invoke(textEditorCommandParameter);

                    var motionedPrimaryImmutableCursor = new ImmutableTextEditorCursor(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor);

                    var startingPositionIndexInclusive = textEditorCommandParameter.TextEditorBase.GetPositionIndex(
                        currentPrimaryImmutableCursor.RowIndex,
                        currentPrimaryImmutableCursor.ColumnIndex);
                    
                    var endingPositionIndexExclusive = textEditorCommandParameter.TextEditorBase.GetPositionIndex(
                        motionedPrimaryImmutableCursor.RowIndex,
                        motionedPrimaryImmutableCursor.ColumnIndex);

                    var cursorForDeletion = new TextEditorCursor(
                        (currentPrimaryImmutableCursor.RowIndex, 
                            currentPrimaryImmutableCursor.ColumnIndex),
                        true);
                    
                    if (startingPositionIndexInclusive > endingPositionIndexExclusive)
                    {
                        (startingPositionIndexInclusive, endingPositionIndexExclusive) = 
                            (endingPositionIndexExclusive, startingPositionIndexInclusive);
                        
                        cursorForDeletion = new TextEditorCursor(
                            (motionedPrimaryImmutableCursor.RowIndex, 
                                motionedPrimaryImmutableCursor.ColumnIndex),
                            true);
                    }

                    var removeCharacterCount = endingPositionIndexExclusive - startingPositionIndexInclusive;
                    
                    var deleteTextTextEditorBaseAction = new DeleteTextByRangeTextEditorBaseAction(
                        textEditorCommandParameter.TextEditorBase.Key,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount,
                        CancellationToken.None);
                
                    textEditorCommandParameter
                        .TextEditorService
                        .DeleteTextByRange(deleteTextTextEditorBaseAction); 
                },
                true,
                displayName,
                displayName);
            
            
        }
        
        return success;
    }
}