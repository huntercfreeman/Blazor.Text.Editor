using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimTextEditorMotionFacts
{
    /// <summary>
    /// Perhaps it is useful to pass in a copy of the user's cursor
    /// when doing a Vim sentence like "dw" or "delete word".
    /// Then dispatch a delete text range action using the starting PositionIndex
    /// and ending PositionIndex
    /// <br/><br/>
    /// This is contrasted with doing a Vim sentence like "w" in which one
    /// just wishes to move the cursor. In this case perhaps it is useful to pass
    /// in the user's cursor directly.
    /// </summary>
    public static void Word(
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
    
    public static void End(
        TextEditorCursor textEditorCursor,
        TextEditorBase textEditorBase,
        bool isRecursiveCall = false)
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
                        
                        End(textEditorCursor, textEditorBase, isRecursiveCall: true);
                        
                        // Leave method early as all is finished.
                        return;
                    }
                }
            }
        }
        
        textEditorCursor.IndexCoordinates = localIndexCoordinates;
        textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;
    }
}