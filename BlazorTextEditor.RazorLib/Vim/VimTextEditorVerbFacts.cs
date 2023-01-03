using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Vim;

public static class VimTextEditorVerbFacts
{
    public static void Delete(
        TextEditorCursor textEditorCursor,
        TextEditorBase textEditorBase)
    {
        var localIndexCoordinates = textEditorCursor.IndexCoordinates;
        var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
        {
            localIndexCoordinates.columnIndex = columnIndex;
            localPreferredColumnIndex = columnIndex;
        }

        textEditorCursor.TextEditorSelection.AnchorPositionIndex = null;

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
    }
}