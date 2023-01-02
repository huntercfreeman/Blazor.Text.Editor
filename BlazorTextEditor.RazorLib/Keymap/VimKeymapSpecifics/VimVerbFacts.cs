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

    public static bool TryParseVimSentence(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        bool verbWasDoubled = false;

        var currentToken = sentenceSnapshot[indexInSentence];

        if (indexInSentence + 1 < sentenceSnapshot.Length)
        {
            var nextToken = sentenceSnapshot[indexInSentence + 1];

            if (nextToken.VimGrammarKind == VimGrammarKind.Verb &&
                nextToken.KeyboardEventArgs.Key == currentToken.KeyboardEventArgs.Key)
            {
                verbWasDoubled = true;
            }
        }

        var success = false;

        if (verbWasDoubled)
        {
            // TODO: When a verb is doubled is it always the case that the position indices to operate over are known without the need of a motion? Example, "dd" would delete the current line and copy it to the in memory clipboard. But no motion was needed to know what text to delete.

            success = true;

            switch (currentToken.KeyboardEventArgs.Key)
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

                    return true;
                }
                case "c":
                {
                    // Delete the current line, enter Vim Insert Mode

                    textEditorCommand = new TextEditorCommand(
                        async textEditorCommandParameter =>
                        {
                            await TextEditorCommandFacts.Cut.DoAsyncFunc
                                .Invoke(textEditorCommandParameter);

                            if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                                is TextEditorKeymapVim vimKeymap)
                            {
                                vimKeymap.ActiveVimMode = VimMode.Insert;
                            }
                        },
                        true,
                        "Vim::Delete(Line)",
                        "Vim::Delete(Line)");

                    return true;
                }
                default:
                {
                    textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
                    return true;
                }
            }
        }
        
        if (keyboardEventArgs.CtrlKey)
        {
            switch (currentToken.KeyboardEventArgs.Key)
            {
                case "e":
                {
                    // Scroll vertically down by the height of a row without moving cursor

                    textEditorCommand = TextEditorCommandFacts.ScrollLineDown;
                    return true;
                }
                case "y":
                {
                    // Scroll vertically up by the height of a row without moving cursor

                    textEditorCommand = TextEditorCommandFacts.ScrollLineUp;

                    return true;
                }
                default:
                {
                    textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
                    return true;
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
            
            switch (currentToken.KeyboardEventArgs.Key)
            {
                case "d":
                case "c":
                {
                    // Delete

                    var displayName = $"Vim::Delete({innerTextEditorCommand.DisplayName})";
                    
                    if (currentToken.KeyboardEventArgs.Key == "c")
                        displayName = $"Vim::Change({innerTextEditorCommand.DisplayName})";

                    textEditorCommand = new TextEditorCommand(
                        async textEditorCommandParameter =>
                        {
                            var textEditorCursorForMotion = new TextEditorCursor(
                                textEditorCommandParameter
                                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates,
                                true);
                            
                            var textEditorCommandParameterForMotion = new TextEditorCommandParameter(
                                textEditorCommandParameter.TextEditorBase,
                                TextEditorCursorSnapshot.TakeSnapshots(textEditorCursorForMotion),
                                textEditorCommandParameter.ClipboardProvider,
                                textEditorCommandParameter.TextEditorService,
                                textEditorCommandParameter.TextEditorViewModel);
                            
                            var motionResult = await VimMotionResult
                                .GetResultAsync(
                                    textEditorCommandParameter,
                                    textEditorCursorForMotion,
                                    async () =>
                                    await innerTextEditorCommand.DoAsyncFunc
                                        .Invoke(textEditorCommandParameterForMotion));

                            var cursorForDeletion = new TextEditorCursor(
                                (motionResult.LowerPositionIndexImmutableCursor.RowIndex,
                                    motionResult.LowerPositionIndexImmutableCursor.ColumnIndex),
                                true);

                            var deleteTextTextEditorBaseAction = new DeleteTextByRangeTextEditorBaseAction(
                                textEditorCommandParameter.TextEditorBase.Key,
                                TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                                motionResult.PositionIndexDisplacement,
                                CancellationToken.None);

                            textEditorCommandParameter
                                .TextEditorService
                                .DeleteTextByRange(deleteTextTextEditorBaseAction);

                            if (currentToken.KeyboardEventArgs.Key == "c" &&
                                textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                                    is TextEditorKeymapVim textEditorKeymapVim)
                            {
                                textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
                            }
                        },
                        true,
                        displayName,
                        displayName);

                    return true;
                }
                default:
                {
                    textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
                    return true;
                }
            }
        }

        return success;
    }
}