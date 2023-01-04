using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap.Vim;

namespace BlazorTextEditor.RazorLib.Commands.Vim;

public static partial class TextEditorCommandVimFacts
{
    public static class Motions
    {
        public static readonly TextEditorCommand Word = new(
            async textEditorCommandParameter =>
            {
                var textEditorCursor = textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor;
                var textEditorBase = textEditorCommandParameter.TextEditorBase;

                var localIndexCoordinates = textEditorCursor.IndexCoordinates;
                var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    localIndexCoordinates.columnIndex = columnIndex;
                    localPreferredColumnIndex = columnIndex;
                }

                var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                if (localIndexCoordinates.columnIndex == lengthOfRow &&
                    localIndexCoordinates.rowIndex < textEditorBase.RowCount - 1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    localIndexCoordinates.rowIndex++;
                }
                else if (localIndexCoordinates.columnIndex != lengthOfRow)
                {
                    var columnIndexOfCharacterWithDifferingKind = textEditorBase
                        .GetColumnIndexOfCharacterWithDifferingKind(
                            localIndexCoordinates.rowIndex,
                            localIndexCoordinates.columnIndex,
                            false);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    else
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(
                            columnIndexOfCharacterWithDifferingKind);
                    }
                }

                textEditorCursor.IndexCoordinates = localIndexCoordinates;
                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

                if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.TextEditorSelection))
                {
                    textEditorCursor.TextEditorSelection.EndingPositionIndex =
                        textEditorBase.GetCursorPositionIndex(textEditorCursor);
                }
                
                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;
            },
            true,
            "Vim::Word()",
            "Vim::Word()");

        public static readonly TextEditorCommand End = new(
            async textEditorCommandParameter =>
                await PerformEnd(textEditorCommandParameter),
            true,
            "Vim::End()",
            "Vim::End()");

        public static async Task PerformEnd(
            ITextEditorCommandParameter textEditorCommandParameter,
            bool isRecursiveCall = false)
        {
            var textEditorCursor = textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor;
            var textEditorBase = textEditorCommandParameter.TextEditorBase;

            var localIndexCoordinates = textEditorCursor.IndexCoordinates;
            var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                localIndexCoordinates.columnIndex = columnIndex;
                localPreferredColumnIndex = columnIndex;
            }

            var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

            if (localIndexCoordinates.columnIndex == lengthOfRow &&
                localIndexCoordinates.rowIndex < textEditorBase.RowCount - 1)
            {
                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                localIndexCoordinates.rowIndex++;
            }
            else if (localIndexCoordinates.columnIndex != lengthOfRow)
            {
                var columnIndexOfCharacterWithDifferingKind = textEditorBase
                    .GetColumnIndexOfCharacterWithDifferingKind(
                        localIndexCoordinates.rowIndex,
                        localIndexCoordinates.columnIndex,
                        false);

                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                }
                else
                {
                    var columnsToMoveBy = columnIndexOfCharacterWithDifferingKind -
                                          localIndexCoordinates.columnIndex;

                    MutateIndexCoordinatesAndPreferredColumnIndex(
                        columnIndexOfCharacterWithDifferingKind);

                    if (columnsToMoveBy > 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(
                            localIndexCoordinates.columnIndex - 1);
                    }
                    else if (columnsToMoveBy == 1 &&
                             !isRecursiveCall)
                    {
                        // Persist state of the first invocation
                        textEditorCursor.IndexCoordinates = localIndexCoordinates;
                        textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

                        var positionIndex = textEditorBase
                            .GetCursorPositionIndex(textEditorCursor);

                        var currentCharacterKind = textEditorBase
                            .GetCharacterKindAt(positionIndex);

                        var nextCharacterKind = textEditorBase
                            .GetCharacterKindAt(positionIndex + 1);

                        if (nextCharacterKind != CharacterKind.Bad &&
                            currentCharacterKind == nextCharacterKind)
                        {
                            /*
                             * If the cursor is at the end of a word.
                             * Then the first End(...) invocation will move the
                             * cursor to the next word.
                             *
                             * One must invoke the End(...) method a second time
                             * however because they will erroneously be at the
                             * start of the next word otherwise.
                             */

                            await PerformEnd(textEditorCommandParameter, isRecursiveCall: true);

                            // Leave method early as all is finished.
                            return;
                        }
                    }
                }
            }

            textEditorCursor.IndexCoordinates = localIndexCoordinates;
            textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;
            
            if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.TextEditorSelection))
            {
                textEditorCursor.TextEditorSelection.EndingPositionIndex =
                    textEditorBase.GetCursorPositionIndex(textEditorCursor);
            }
        }

        public static readonly TextEditorCommand Back = new(
            async textEditorCommandParameter =>
            {
                var textEditorCursor = textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor;
                var textEditorBase = textEditorCommandParameter.TextEditorBase;

                var localIndexCoordinates = textEditorCursor.IndexCoordinates;
                var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    localIndexCoordinates.columnIndex = columnIndex;
                    localPreferredColumnIndex = columnIndex;
                }

                if (localIndexCoordinates.columnIndex == 0)
                {
                    if (localIndexCoordinates.rowIndex != 0)
                    {
                        localIndexCoordinates.rowIndex--;

                        var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                    }
                }
                else
                {
                    var columnIndexOfCharacterWithDifferingKind = textEditorBase
                        .GetColumnIndexOfCharacterWithDifferingKind(
                            localIndexCoordinates.rowIndex,
                            localIndexCoordinates.columnIndex,
                            true);

                    if (columnIndexOfCharacterWithDifferingKind == -1)
                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                    else
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(
                            columnIndexOfCharacterWithDifferingKind);
                    }
                }

                textEditorCursor.IndexCoordinates = localIndexCoordinates;
                textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;
                
                if (TextEditorSelectionHelper.HasSelectedText(textEditorCursor.TextEditorSelection))
                {
                    textEditorCursor.TextEditorSelection.EndingPositionIndex =
                        textEditorBase.GetCursorPositionIndex(textEditorCursor);
                }
            },
            true,
            "Vim::Back()",
            "Vim::Back()");

        public static TextEditorCommand GetVisual(TextEditorCommand textEditorCommandMotion, string displayName)
        {
            return new TextEditorCommand(
                async textEditorCommandParameter =>
                {
                    if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap is not
                        TextEditorKeymapVim keymapVim)
                        return;

                    var previousAnchorPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex;

                    var previousEndingPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;

                    await textEditorCommandMotion.DoAsyncFunc.Invoke(textEditorCommandParameter);
                    
                    var nextEndingPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;
                    
                    Console.WriteLine($"previousEndingPositionIndex: {previousEndingPositionIndex}");
                    Console.WriteLine($"nextEndingPositionIndex: {nextEndingPositionIndex}");

                    if (nextEndingPositionIndex < textEditorCommandParameter
                            .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                    {
                        if (previousAnchorPositionIndex < previousEndingPositionIndex)
                        {
                            // Anchor went from being the lower bound to the upper bound.

                            textEditorCommandParameter
                                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex += 1;
                        }
                    }
                    else if (nextEndingPositionIndex >= textEditorCommandParameter
                                 .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                    {
                        if (previousAnchorPositionIndex > previousEndingPositionIndex)
                        {
                            // Anchor went from being the upper bound to the lower bound.

                            textEditorCommandParameter
                                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex -= 1;
                        }

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                            .EndingPositionIndex += 1;
                    }
                },
                true,
                $"Vim::GetVisual({displayName})",
                $"Vim::GetVisual({displayName})");
        }

        public static TextEditorCommand GetVisualLine(TextEditorCommand textEditorCommandMotion, string displayName)
        {
            return new TextEditorCommand(
                async textEditorCommandParameter =>
                {
                    if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap is not
                        TextEditorKeymapVim keymapVim)
                        return;

                    var previousAnchorPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex;

                    var previousEndingPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;

                    await textEditorCommandMotion.DoAsyncFunc.Invoke(textEditorCommandParameter);

                    var nextEndingPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;

                    if (nextEndingPositionIndex < textEditorCommandParameter
                            .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                    {
                        if (previousAnchorPositionIndex < previousEndingPositionIndex)
                        {
                            // Anchor went from being the lower bound to the upper bound.

                            var rowDataAnchorIsOn = textEditorCommandParameter.TextEditorBase
                                .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                                    previousAnchorPositionIndex.Value);

                            textEditorCommandParameter
                                    .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                                textEditorCommandParameter
                                    .TextEditorBase.RowEndingPositions[rowDataAnchorIsOn.rowIndex]
                                    .positionIndex;
                        }

                        var startingPositionOfRow = textEditorCommandParameter.TextEditorBase
                            .GetStartOfRowTuple(textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor
                                .IndexCoordinates.rowIndex)
                            .positionIndex;

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                            .EndingPositionIndex = startingPositionOfRow;
                    }
                    else if (nextEndingPositionIndex >= textEditorCommandParameter
                                 .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                    {
                        if (previousAnchorPositionIndex > previousEndingPositionIndex)
                        {
                            // Anchor went from being the upper bound to the lower bound.

                            var rowDataAnchorIsOn = textEditorCommandParameter.TextEditorBase
                                .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                                    previousAnchorPositionIndex.Value);

                            textEditorCommandParameter
                                    .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                                textEditorCommandParameter.TextEditorBase.GetStartOfRowTuple(
                                        rowDataAnchorIsOn.rowIndex - 1)
                                    .positionIndex;
                        }

                        var endingPositionOfRow = textEditorCommandParameter
                            .TextEditorBase.RowEndingPositions[
                                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex]
                            .positionIndex;

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                            .EndingPositionIndex = endingPositionOfRow;
                    }
                },
                true,
                $"Vim::GetVisual({displayName})",
                $"Vim::GetVisual({displayName})");
        }
    }
}