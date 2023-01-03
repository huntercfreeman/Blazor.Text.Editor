using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Vim;

public static class VimTextEditorMotionFacts
{
    public static void End(
        TextEditorCursor textEditorCursor,
        TextEditorBase textEditorBase,
        bool isRecursiveCall = false)
    {
    }
    
    public static void Back(
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
    }
}