using System.Collections.Immutable;
using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands.Default;

public static class TextEditorCommandDefaultFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        _ => Task.CompletedTask,
        false,
        "DoNothingDiscard",
        "defaults_do-nothing-discard");

    public static readonly TextEditorCommand Copy = new(
        async commandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    commandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableSelection,
                    commandParameter.Model);

            selectedText ??= commandParameter.Model.GetLinesRange(
                    commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);
            
            await commandParameter
                .ClipboardService
                .SetClipboard(
                    selectedText);

            await commandParameter.ViewModel.FocusAsync();
        },
        false,
        "Copy",
        "defaults_copy");
    
    public static readonly TextEditorCommand Cut = new(
        async commandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    commandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableSelection,
                    commandParameter.Model);

            var cursorSnapshots = commandParameter.CursorSnapshots;
            
            if (selectedText is null)
            {
                var cursor = TextEditorSelectionHelper.SelectLinesRange(
                    commandParameter.Model,
                    commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                cursorSnapshots = TextEditorCursorSnapshot
                    .TakeSnapshots(cursor);

                var primaryCursorSnapshot = cursorSnapshots.FirstOrDefault();

                if (primaryCursorSnapshot is null)
                    return; // Should never occur
                
                selectedText = TextEditorSelectionHelper
                    .GetSelectedText(
                        primaryCursorSnapshot.ImmutableCursor.ImmutableSelection,
                        commandParameter.Model);
            }

            if (selectedText is null)
                return; // Should never occur
            
            await commandParameter
                .ClipboardService
                .SetClipboard(
                    selectedText);

            await commandParameter.ViewModel.FocusAsync();
            
            commandParameter.TextEditorService.Model
                .HandleKeyboardEvent(new TextEditorModelsCollection.KeyboardEventAction(
                    commandParameter.Model.ModelKey,
                    cursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MetaKeys.DELETE
                    },
                    CancellationToken.None));
        },
        true,
        "Cut",
        "defaults_cut");

    public static readonly TextEditorCommand Paste = new(
        async commandParameter =>
        {
            var clipboard = await commandParameter
                .ClipboardService
                .ReadClipboard();

            commandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    commandParameter.Model.ModelKey,
                    new[]
                    {
                        commandParameter.PrimaryCursorSnapshot,
                    }.ToImmutableArray(),
                    clipboard,
                    CancellationToken.None));
        },
        true,
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly TextEditorCommand Save = new(
        commandParameter =>
        {
            var onSaveRequestedFunc = commandParameter
                .ViewModel
                .OnSaveRequested; 
            
            if (onSaveRequestedFunc is not null)
            {
                onSaveRequestedFunc
                    .Invoke(commandParameter.Model);
                
                commandParameter.TextEditorService.ViewModel.With(
                    commandParameter.ViewModel.ViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        TextEditorStateChangedKey = TextEditorStateChangedKey.NewTextEditorStateChangedKey()
                    });
            }
            
            return Task.CompletedTask;
        },
        false,
        "Save",
        "defaults_save");

    public static readonly TextEditorCommand SelectAll = new(
        commandParameter =>
        {
            var primaryCursor = commandParameter
                .PrimaryCursorSnapshot.UserCursor;

            primaryCursor.Selection.AnchorPositionIndex =
                0;

            primaryCursor.Selection.EndingPositionIndex =
                commandParameter.Model.DocumentLength;
            
            commandParameter.TextEditorService.ViewModel.With(
                commandParameter.ViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    TextEditorStateChangedKey = TextEditorStateChangedKey.NewTextEditorStateChangedKey()
                });
            
            return Task.CompletedTask;
        },
        false,
        "Select All",
        "defaults_select-all");
    
    public static readonly TextEditorCommand Undo = new(
        commandParameter =>
        {
            commandParameter.TextEditorService.Model
                .UndoEdit(commandParameter.Model.ModelKey);
            
            return Task.CompletedTask;
        },
        true,
        "Undo",
        "defaults_undo");
    
    public static readonly TextEditorCommand Redo = new(
        commandParameter =>
        {
            commandParameter.TextEditorService.Model
                .RedoEdit(commandParameter.Model.ModelKey);

            return Task.CompletedTask;
        },
        true,
        "Redo",
        "defaults_redo");
    
    public static readonly TextEditorCommand Remeasure = new(
        commandParameter =>
        {
            commandParameter.TextEditorService.ViewModel.With(
                commandParameter.ViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    ShouldMeasureDimensions = true,
                    TextEditorStateChangedKey = TextEditorStateChangedKey.NewTextEditorStateChangedKey()
                });
            
            commandParameter.TextEditorService.ViewModel.With(
                commandParameter.ViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    TextEditorStateChangedKey = TextEditorStateChangedKey.NewTextEditorStateChangedKey()
                });
            
            return Task.CompletedTask;
        },
        false,
        "Remeasure",
        "defaults_remeasure");
    
    public static readonly TextEditorCommand ScrollLineDown = new(
        async commandParameter =>
        {
            await commandParameter.ViewModel
                .MutateScrollVerticalPositionByLinesAsync(1);
        },
        false,
        "Scroll Line Down",
        "defaults_scroll-line-down");
    
    public static readonly TextEditorCommand ScrollLineUp = new(
        async commandParameter =>
        {
            await commandParameter.ViewModel
                .MutateScrollVerticalPositionByLinesAsync(-1);
        },
        false,
        "Scroll Line Up",
        "defaults_scroll-line-up");
    
    public static readonly TextEditorCommand ScrollPageDown = new(
        async commandParameter =>
        {
            await commandParameter.ViewModel
                .MutateScrollVerticalPositionByPagesAsync(1);
        },
        false,
        "Scroll Page Down",
        "defaults_scroll-page-down");
    
    public static readonly TextEditorCommand ScrollPageUp = new(
        async commandParameter =>
        {
            await commandParameter.ViewModel
                .MutateScrollVerticalPositionByPagesAsync(-1);
        },
        false,
        "Scroll Page Up",
        "defaults_scroll-page-up");
    
    public static readonly TextEditorCommand CursorMovePageBottom = new(
        commandParameter =>
        {
            commandParameter.ViewModel
                .CursorMovePageBottom();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Bottom of the Page",
        "defaults_cursor-move-page-bottom");
    
    public static readonly TextEditorCommand CursorMovePageTop = new(
        commandParameter =>
        {
            commandParameter.ViewModel
                .CursorMovePageTop();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Top of the Page",
        "defaults_cursor-move-page-top");
    
    public static readonly TextEditorCommand Duplicate = new(
        commandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    commandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableSelection,
                    commandParameter.Model);

            TextEditorCursor cursorForInsertion;

            if (selectedText is null)
            {
                selectedText = commandParameter.Model.GetLinesRange(
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        1);
                
                cursorForInsertion = new TextEditorCursor(
                    (commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        0),
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            else
            {
                // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
                // selected text to duplicate would be overwritten by itself and do nothing
                cursorForInsertion = new TextEditorCursor(
                    (commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex),
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            
            var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
                commandParameter.Model.ModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                selectedText,
                CancellationToken.None);
                
            commandParameter.TextEditorService.Model
                .InsertText(insertTextAction);

            return Task.CompletedTask;
        },
        false,
        "Duplicate",
        "defaults_duplicate");
    
    public static readonly TextEditorCommand IndentMore = new(
        commandParameter =>
        {
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper
                .GetSelectionBounds(
                    commandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper
                .ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                    commandParameter.Model,
                    selectionBoundsInPositionIndexUnits);

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var cursorForInsertion = new TextEditorCursor(
                    (i, 0),
                    true);
                
                var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
                    commandParameter.Model.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                    KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                    CancellationToken.None);
                
                commandParameter.TextEditorService.Model
                    .InsertText(insertTextAction);
            }

            var lowerBoundPositionIndexChange = 1;
            
            var upperBoundPositionIndexChange = 
                selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
            
            if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    lowerBoundPositionIndexChange;
                
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    upperBoundPositionIndexChange;
            }
            else
            {
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex +=
                    upperBoundPositionIndexChange;
                
                commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex +=
                    lowerBoundPositionIndexChange;
            }

            var userCursorIndexCoordinates =
                commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (userCursorIndexCoordinates.rowIndex,
                    userCursorIndexCoordinates.columnIndex + 1);

            return Task.CompletedTask;
        },
        false,
        "Indent More",
        "defaults_indent-more");
    
    public static readonly TextEditorCommand IndentLess = new(
        commandParameter =>
        {
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper
                .GetSelectionBounds(
                    commandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper
                .ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                    commandParameter.Model,
                    selectionBoundsInPositionIndexUnits);

            bool isFirstLoop = true;
            
            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var rowPositionIndex = commandParameter.Model
                    .GetPositionIndex(
                        i,
                        0);

                var characterReadCount = TextEditorModel.TAB_WIDTH;

                var lengthOfRow = commandParameter.Model.GetLengthOfRow(i);

                characterReadCount = Math.Min(lengthOfRow, characterReadCount);

                var readResult =
                    commandParameter.Model
                        .GetTextRange(rowPositionIndex, characterReadCount);

                int removeCharacterCount = 0;
                
                if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
                {
                    removeCharacterCount = 1;
                    
                    var cursorForDeletion = new TextEditorCursor(
                        (i, 0),
                        true);
                    
                    var deleteTextAction = new TextEditorModelsCollection.DeleteTextByRangeAction(
                        commandParameter.Model.ModelKey,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount, // Delete a single "Tab" character
                        CancellationToken.None);
                
                    commandParameter
                        .TextEditorService.Model
                        .DeleteTextByRange(deleteTextAction);
                }
                else if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.SPACE))
                {
                    var cursorForDeletion = new TextEditorCursor(
                        (i, 0),
                        true);

                    var contiguousSpaceCount = 0;
                    
                    foreach (var character in readResult)
                    {
                        if (character == KeyboardKeyFacts.WhitespaceCharacters.SPACE)
                            contiguousSpaceCount++;
                    }

                    removeCharacterCount = contiguousSpaceCount;
                    
                    var deleteTextAction = new TextEditorModelsCollection.DeleteTextByRangeAction(
                        commandParameter.Model.ModelKey,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount,
                        CancellationToken.None);
                
                    commandParameter
                        .TextEditorService.Model
                        .DeleteTextByRange(deleteTextAction);
                }

                // Modify the lower bound of user's text selection
                if (isFirstLoop)
                {
                    isFirstLoop = false;
                    
                    if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                    {
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                            removeCharacterCount;
                    }
                    else
                    {
                        commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                            removeCharacterCount;
                    }
                }
                
                // Modify the upper bound of user's text selection
                if (commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex <
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex)
                {
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex -=
                        removeCharacterCount;
                }
                
                // Modify the column index of user's cursor
                if (i == commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex)
                {
                    var nextColumnIndex = commandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex -
                                          removeCharacterCount;

                    var userCursorIndexCoordinates = commandParameter
                        .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
                    
                    commandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                        (userCursorIndexCoordinates.rowIndex, Math.Max(0, nextColumnIndex));
                }
            }

            return Task.CompletedTask;
        },
        false,
        "Indent Less",
        "defaults_indent-less");
    
    public static readonly TextEditorCommand ClearTextSelection = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            
            return Task.CompletedTask;
        },
        false,
        "ClearTextSelection",
        "defaults_clear-text-selection");
    
    public static readonly TextEditorCommand NewLineBelow = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            
            var lengthOfRow = textEditorCommandParameter.Model.GetLengthOfRow(
                textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex);

            var temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, lengthOfRow);
            
            textEditorCommandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.Model.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));
            
            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");
    
    public static readonly TextEditorCommand NewLineAbove = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            
            var temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, 0);
            
            textEditorCommandParameter.TextEditorService.Model.InsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.Model.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));
            
            temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            if (temporaryIndexCoordinates.rowIndex > 1)
            {
                textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (temporaryIndexCoordinates.rowIndex - 1, 0);
            }
            
            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");
    
    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        textEditorCommandParameter =>
        {
            var cursorPositionIndex = textEditorCommandParameter.Model.GetCursorPositionIndex(
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor);
            
            if (shouldSelectText)
            {
                if (!TextEditorSelectionHelper.HasSelectedText(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.Selection))
                {
                    textEditorCommandParameter
                            .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                        cursorPositionIndex;
                }
            }
            else
            {
                textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            }
            
            var previousCharacter = textEditorCommandParameter.Model.GetTextAt(
                cursorPositionIndex - 1);
            
            var currentCharacter = textEditorCommandParameter.Model.GetTextAt(
                cursorPositionIndex);

            char? characterToMatch = null;
            char? match = null;
            var fallbackToPreviousCharacter = false;

            if (CharacterKindHelper.CharToCharacterKind(currentCharacter) == CharacterKind.Punctuation)
            {
                // Prefer current character
                match = KeyboardKeyFacts
                    .MatchPunctuationCharacter(currentCharacter);

                if (match is not null)
                    characterToMatch = currentCharacter;
            }
            
            if (characterToMatch is null &&
                CharacterKindHelper.CharToCharacterKind(previousCharacter) == CharacterKind.Punctuation)
            {
                // Fallback to the previous current character
                match = KeyboardKeyFacts
                    .MatchPunctuationCharacter(previousCharacter);
                
                if (match is not null)
                {
                    characterToMatch = previousCharacter;
                    fallbackToPreviousCharacter = true;
                }
            } 

            if (characterToMatch is null)
                return Task.CompletedTask;
            
            if (match is null)
                return Task.CompletedTask;

            var directionToFindMatchMatchingPunctuationCharacter = KeyboardKeyFacts
                .DirectionToFindMatchMatchingPunctuationCharacter(characterToMatch.Value);

            if (directionToFindMatchMatchingPunctuationCharacter is null)
                return Task.CompletedTask;

            var temporaryCursor = new TextEditorCursor(
                (textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex,
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.columnIndex),
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);

            var unmatchedCharacters =
                (fallbackToPreviousCharacter &&
                 directionToFindMatchMatchingPunctuationCharacter == -1)
                    ? 0
                    : 1;

            while (true)
            {
                KeyboardEventArgs keyboardEventArgs;

                if (directionToFindMatchMatchingPunctuationCharacter == -1)
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                    };
                }
                else
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    };
                }
                
                TextEditorCursor.MoveCursor(
                    keyboardEventArgs,
                    temporaryCursor,
                    textEditorCommandParameter.Model);

                var temporaryCursorPositionIndex = textEditorCommandParameter.Model
                    .GetCursorPositionIndex(
                        temporaryCursor);
                
                var characterAt = textEditorCommandParameter.Model.GetTextAt(
                    temporaryCursorPositionIndex);
                
                if (characterAt == match)
                    unmatchedCharacters--;
                else if (characterAt == characterToMatch)
                    unmatchedCharacters++;

                if (unmatchedCharacters == 0)
                    break;

                if (temporaryCursorPositionIndex <= 0 ||
                    temporaryCursorPositionIndex >= textEditorCommandParameter.Model.DocumentLength)
                    break;
            }
            
            if (shouldSelectText)
            {
                textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                    textEditorCommandParameter.Model.GetCursorPositionIndex(temporaryCursor);
            }
 
            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                temporaryCursor.IndexCoordinates;
            
            return Task.CompletedTask;
        },
        true,
        "GoToMatchingCharacter",
        "defaults_go-to-matching-character");
    
    public static readonly TextEditorCommand GoToDefinition = new(
        textEditorCommandParameter =>
        {
            if (textEditorCommandParameter.Model.SemanticModel is null)
                return Task.CompletedTask;

            var positionIndex = textEditorCommandParameter.Model
                .GetCursorPositionIndex(
                    textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor);
            
            var textSpanOfWordAtPositionIndex = textEditorCommandParameter.Model
                .GetWordAt(positionIndex);
            
            if (textSpanOfWordAtPositionIndex is null)
                return Task.CompletedTask;

            var symbolDefinition = textEditorCommandParameter.Model.SemanticModel
                .GoToDefinition(
                    textEditorCommandParameter.Model,
                    textSpanOfWordAtPositionIndex);

            if (symbolDefinition is not null)
            {
                var rowInformation = textEditorCommandParameter.Model
                    .FindRowInformation(symbolDefinition.PositionIndex);
                
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (rowInformation.rowIndex, 
                        symbolDefinition.PositionIndex - rowInformation.rowStartPositionIndex);
            }

            return Task.CompletedTask;
        },
        false,
        "GoToDefinition",
        "defaults_go-to-definition");
}