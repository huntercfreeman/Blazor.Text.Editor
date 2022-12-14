using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Cursor;

public class TextEditorCursor
{
    public TextEditorCursor(bool isPrimaryCursor)
    {
        IsPrimaryCursor = isPrimaryCursor;
    }

    public TextEditorCursor((int rowIndex, int columnIndex) rowAndColumnIndex, bool isPrimaryCursor)
        : this(isPrimaryCursor)
    {
        IndexCoordinates = rowAndColumnIndex;
        PreferredColumnIndex = IndexCoordinates.columnIndex;
    }

    public (int rowIndex, int columnIndex) IndexCoordinates { get; set; }
    public int PreferredColumnIndex { get; set; }
    public TextEditorSelection TextEditorSelection { get; } = new();
    public bool ShouldRevealCursor { get; set; }
    public bool IsPrimaryCursor { get; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool IsIntersecting { get; set; }

    public static void MoveCursor(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorCursor textEditorCursor,
        TextEditorBase textEditorBase)
    {
        var localIndexCoordinates = textEditorCursor.IndexCoordinates;
        var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

        var rememberTextEditorSelection = new TextEditorSelection
        {
            AnchorPositionIndex = textEditorCursor.TextEditorSelection.AnchorPositionIndex,
            EndingPositionIndex = textEditorCursor.TextEditorSelection.EndingPositionIndex,
        };

        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
        {
            localIndexCoordinates.columnIndex = columnIndex;
            localPreferredColumnIndex = columnIndex;
        }

        if (keyboardEventArgs.ShiftKey)
        {
            if (textEditorCursor.TextEditorSelection.AnchorPositionIndex is null ||
                textEditorCursor.TextEditorSelection.EndingPositionIndex ==
                textEditorCursor.TextEditorSelection.AnchorPositionIndex)
            {
                var positionIndex = textEditorBase.GetPositionIndex(
                    localIndexCoordinates.rowIndex,
                    localIndexCoordinates.columnIndex);

                textEditorCursor.TextEditorSelection.AnchorPositionIndex = positionIndex;
            }
        }
        else
            textEditorCursor.TextEditorSelection.AnchorPositionIndex = null;

        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            {
                if (TextEditorSelectionHelper.HasSelectedText(rememberTextEditorSelection) &&
                    !keyboardEventArgs.ShiftKey)
                {
                    var selectionBounds = TextEditorSelectionHelper
                        .GetSelectionBounds(rememberTextEditorSelection);

                    var lowerRowMetaData = textEditorBase
                        .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                            selectionBounds.lowerPositionIndexInclusive);

                    localIndexCoordinates.rowIndex =
                        lowerRowMetaData.rowIndex;

                    localIndexCoordinates.columnIndex =
                        selectionBounds.lowerPositionIndexInclusive - lowerRowMetaData.rowStartPositionIndex;
                }
                else
                {
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
                        if (keyboardEventArgs.CtrlKey)
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
                        else
                            MutateIndexCoordinatesAndPreferredColumnIndex(localIndexCoordinates.columnIndex - 1);
                    }
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            {
                if (localIndexCoordinates.rowIndex < textEditorBase.RowCount - 1)
                {
                    localIndexCoordinates.rowIndex++;

                    var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                    localIndexCoordinates.columnIndex = lengthOfRow < localPreferredColumnIndex
                        ? lengthOfRow
                        : localPreferredColumnIndex;
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            {
                if (localIndexCoordinates.rowIndex > 0)
                {
                    localIndexCoordinates.rowIndex--;

                    var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                    localIndexCoordinates.columnIndex = lengthOfRow < localPreferredColumnIndex
                        ? lengthOfRow
                        : localPreferredColumnIndex;
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            {
                if (TextEditorSelectionHelper
                        .HasSelectedText(rememberTextEditorSelection) &&
                    !keyboardEventArgs.ShiftKey)
                {
                    var selectionBounds = TextEditorSelectionHelper
                        .GetSelectionBounds(rememberTextEditorSelection);

                    var upperRowMetaData =
                        textEditorBase
                            .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                                selectionBounds.upperPositionIndexExclusive);

                    localIndexCoordinates.rowIndex =
                        upperRowMetaData.rowIndex;

                    if (localIndexCoordinates.rowIndex >= textEditorBase.RowCount)
                    {
                        localIndexCoordinates.rowIndex = textEditorBase.RowCount - 1;

                        var upperRowLength = textEditorBase
                            .GetLengthOfRow(localIndexCoordinates.rowIndex);

                        localIndexCoordinates.columnIndex = upperRowLength;
                    }
                    else
                    {
                        localIndexCoordinates.columnIndex =
                            selectionBounds.upperPositionIndexExclusive -
                            upperRowMetaData.rowStartPositionIndex;
                    }
                }
                else
                {
                    var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                    if (localIndexCoordinates.columnIndex == lengthOfRow &&
                        localIndexCoordinates.rowIndex < textEditorBase.RowCount - 1)
                    {
                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                        localIndexCoordinates.rowIndex++;
                    }
                    else if (localIndexCoordinates.columnIndex != lengthOfRow)
                    {
                        if (keyboardEventArgs.CtrlKey)
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
                        else
                            MutateIndexCoordinatesAndPreferredColumnIndex(localIndexCoordinates.columnIndex + 1);
                    }
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.HOME:
            {
                if (keyboardEventArgs.CtrlKey)
                    localIndexCoordinates.rowIndex = 0;

                MutateIndexCoordinatesAndPreferredColumnIndex(0);

                break;
            }
            case KeyboardKeyFacts.MovementKeys.END:
            {
                if (keyboardEventArgs.CtrlKey)
                    localIndexCoordinates.rowIndex = textEditorBase.RowCount - 1;

                var lengthOfRow = textEditorBase.GetLengthOfRow(localIndexCoordinates.rowIndex);

                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);

                break;
            }
        }

        textEditorCursor.IndexCoordinates = localIndexCoordinates;
        textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

        if (keyboardEventArgs.ShiftKey)
        {
            var positionIndex = textEditorBase.GetPositionIndex(
                localIndexCoordinates.rowIndex,
                localIndexCoordinates.columnIndex);

            textEditorCursor.TextEditorSelection.EndingPositionIndex = positionIndex;
        }
    }
}