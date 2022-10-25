using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorBase
{
    private void EnsureUndoPoint(
        TextEditKind textEditKind,
        string? otherTextEditKindIdentifier = null)
    {
        if (textEditKind == TextEditKind.Other &&
            otherTextEditKindIdentifier is null)
        {
            TextEditorCommand.ThrowOtherTextEditKindIdentifierWasExpectedException(
                textEditKind);
        }
        
        var mostRecentEditBlock = _editBlocks.LastOrDefault();

        if (mostRecentEditBlock is null ||
            mostRecentEditBlock.TextEditKind != textEditKind)
        {
            _editBlocks.Add(new EditBlock(
                textEditKind,
                textEditKind.ToString(),
                GetAllText(),
                otherTextEditKindIdentifier));
        }

        while (_editBlocks.Count > MaximumEditBlocks &&
               _editBlocks.Count != 0)
        {
            _editBlocks.RemoveAt(0);
        }
    }

    private void PerformInsertions(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        EnsureUndoPoint(TextEditKind.Insertion);

        foreach (var cursorSnapshot in editTextEditorBaseAction.CursorSnapshots)
        {
            var startOfRowPositionIndex =
                GetStartOfRowTuple(cursorSnapshot.ImmutableCursor.RowIndex)
                    .positionIndex;

            var cursorPositionIndex =
                startOfRowPositionIndex + cursorSnapshot.ImmutableCursor.ColumnIndex;

            var wasTabCode = false;
            var wasEnterCode = false;

            var characterValueToInsert = editTextEditorBaseAction.KeyboardEventArgs.Key
                .First();

            if (KeyboardKeyFacts.IsWhitespaceCode(editTextEditorBaseAction.KeyboardEventArgs.Code))
            {
                characterValueToInsert =
                    KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(editTextEditorBaseAction.KeyboardEventArgs.Code);

                wasTabCode = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE ==
                             editTextEditorBaseAction.KeyboardEventArgs.Code;

                wasEnterCode = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE ==
                               editTextEditorBaseAction.KeyboardEventArgs.Code;
            }

            var characterCountInserted = 1;

            if (wasEnterCode)
            {
                var rowEndingKindToInsert = UsingRowEndingKind;
                
                var richCharacters = rowEndingKindToInsert
                    .AsCharacters()
                    .Select(character => new RichCharacter
                    {
                        Value = character,
                        DecorationByte = default(byte)
                    });
                
                characterCountInserted = rowEndingKindToInsert.AsCharacters().Length;
                
                _content.InsertRange(cursorPositionIndex, richCharacters);
                
                _rowEndingPositions.Insert(cursorSnapshot.ImmutableCursor.RowIndex,
                    (cursorPositionIndex + characterCountInserted, rowEndingKindToInsert));

                MutateRowEndingKindCount(
                    UsingRowEndingKind, 
                    1);
                
                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;

                cursorSnapshot.UserCursor.IndexCoordinates = (indexCoordinates.rowIndex + 1, 0);

                cursorSnapshot.UserCursor.PreferredColumnIndex =
                    cursorSnapshot.UserCursor.IndexCoordinates.columnIndex;
            }
            else
            {
                if (wasTabCode)
                {
                    var index = _tabKeyPositions
                        .FindIndex(x =>
                            x >= cursorPositionIndex);

                    if (index == -1)
                    {
                        _tabKeyPositions.Add(cursorPositionIndex);
                    }
                    else
                    {
                        for (int i = index; i < _tabKeyPositions.Count; i++)
                        {
                            _tabKeyPositions[i]++;
                        }

                        _tabKeyPositions.Insert(index, cursorPositionIndex);
                    }
                }
                
                var richCharacterToInsert = new RichCharacter
                {
                    Value = characterValueToInsert,
                    DecorationByte = default(byte)
                };

                _content.Insert(cursorPositionIndex, richCharacterToInsert);

                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;

                cursorSnapshot.UserCursor.IndexCoordinates =
                    (indexCoordinates.rowIndex, indexCoordinates.columnIndex + 1);
                cursorSnapshot.UserCursor.PreferredColumnIndex =
                    cursorSnapshot.UserCursor.IndexCoordinates.columnIndex;
            }

            var firstRowIndexToModify = wasEnterCode
                ? cursorSnapshot.ImmutableCursor.RowIndex + 1
                : cursorSnapshot.ImmutableCursor.RowIndex;

            for (int i = firstRowIndexToModify; i < _rowEndingPositions.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositions[i];

                _rowEndingPositions[i] = (rowEndingTuple.positionIndex + characterCountInserted, rowEndingTuple.rowEndingKind);
            }

            if (!wasTabCode)
            {
                var firstTabKeyPositionIndexToModify = _tabKeyPositions
                    .FindIndex(x => x >= cursorPositionIndex);

                if (firstTabKeyPositionIndexToModify != -1)
                {
                    for (int i = firstTabKeyPositionIndexToModify; i < _tabKeyPositions.Count; i++)
                    {
                        _tabKeyPositions[i] += characterCountInserted;
                    }
                }
            }
        }
    }

    private void PerformDeletions(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        EnsureUndoPoint(TextEditKind.Deletion);

        foreach (var cursorSnapshot in editTextEditorBaseAction.CursorSnapshots)
        {
            var startOfRowPositionIndex =
                GetStartOfRowTuple(cursorSnapshot.ImmutableCursor.RowIndex)
                    .positionIndex;

            var cursorPositionIndex =
                startOfRowPositionIndex + cursorSnapshot.ImmutableCursor.ColumnIndex;

            int startingPositionIndexToRemoveInclusive;
            int countToRemove;
            bool moveBackwards;

            // Cannot calculate this after text was deleted - it would be wrong
            int? selectionUpperBoundRowIndex = null;
            // Needed for when text selection is deleted
            (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

            if (TextEditorSelectionHelper.HasSelectedText(
                    cursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection))
            {
                var lowerBound = cursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection
                    .AnchorPositionIndex ?? 0; 
                
                var upperBound = cursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection
                    .EndingPositionIndex;

                if (lowerBound > upperBound)
                    (lowerBound, upperBound) = (upperBound, lowerBound);
                
                var lowerRowMetaData = 
                    FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                        lowerBound);
                
                var upperRowMetaData = 
                    FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                        upperBound);

                // Value is needed when knowing what row ending positions
                // to update after deletion is done
                selectionUpperBoundRowIndex = upperRowMetaData.rowIndex;
                
                // Value is needed when knowing where to position the
                // cursor after deletion is done
                selectionLowerBoundIndexCoordinates = 
                    (lowerRowMetaData.rowIndex,
                        lowerBound - lowerRowMetaData.rowStartPositionIndex);

                startingPositionIndexToRemoveInclusive = upperBound - 1;
                countToRemove = upperBound - lowerBound;
                moveBackwards = true;

                cursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            }
            else if (KeyboardKeyFacts.MetaKeys.BACKSPACE == editTextEditorBaseAction.KeyboardEventArgs.Key)
            {
                startingPositionIndexToRemoveInclusive = cursorPositionIndex - 1;
                countToRemove = 1;
                moveBackwards = true;
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == editTextEditorBaseAction.KeyboardEventArgs.Key)
            {
                startingPositionIndexToRemoveInclusive = cursorPositionIndex;
                countToRemove = 1;
                moveBackwards = false;
            }
            else
            {
                throw new ApplicationException(
                    $"The keyboard key: {editTextEditorBaseAction.KeyboardEventArgs.Key} was not recognized");
            }

            int charactersRemovedCount = 0;
            int rowsRemovedCount = 0;

            var indexToRemove = startingPositionIndexToRemoveInclusive;

            while (countToRemove-- > 0)
            {
                if (indexToRemove < 0 ||
                    indexToRemove > _content.Count - 1)
                {
                    break;
                }

                var characterToDelete = _content[indexToRemove];

                int startingIndexToRemoveRange;
                int countToRemoveRange;

                if (KeyboardKeyFacts.IsLineEndingCharacter(characterToDelete.Value))
                {
                    rowsRemovedCount++;

                    // rep.positionIndex == indexToRemove + 1
                    //     ^is for backspace
                    //
                    // rep.positionIndex == indexToRemove + 2
                    //     ^is for delete
                    var rowEndingTupleIndex = _rowEndingPositions
                        .FindIndex(rep =>
                            rep.positionIndex == indexToRemove + 1 ||
                            rep.positionIndex == indexToRemove + 2);

                    var rowEndingTuple = _rowEndingPositions[rowEndingTupleIndex];

                    _rowEndingPositions.RemoveAt(rowEndingTupleIndex);

                    var lengthOfRowEnding = rowEndingTuple.rowEndingKind
                        .AsCharacters().Length;

                    if (moveBackwards)
                    {
                        startingIndexToRemoveRange = indexToRemove - (lengthOfRowEnding - 1);
                    }
                    else
                    {
                        startingIndexToRemoveRange = indexToRemove;
                    }

                    countToRemove -= (lengthOfRowEnding - 1);
                    countToRemoveRange = lengthOfRowEnding;
                    
                    MutateRowEndingKindCount(
                        rowEndingTuple.rowEndingKind, 
                        -1);
                }
                else
                {
                    if (characterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                        _tabKeyPositions.Remove(indexToRemove);

                    startingIndexToRemoveRange = indexToRemove;
                    countToRemoveRange = 1;
                }

                charactersRemovedCount += countToRemoveRange;

                _content.RemoveRange(startingIndexToRemoveRange, countToRemoveRange);

                if (moveBackwards)
                    indexToRemove -= countToRemoveRange;
            }
            
            if (charactersRemovedCount == 0 &&
                rowsRemovedCount == 0)
            {
                return;
            }
            
            if (moveBackwards && !selectionUpperBoundRowIndex.HasValue)
            {
                var modifyRowsBy = -1 * rowsRemovedCount;

                var startOfCurrentRowPositionIndex = GetStartOfRowTuple(
                        cursorSnapshot.ImmutableCursor.RowIndex + modifyRowsBy)
                    .positionIndex;

                var modifyPositionIndexBy = -1 * charactersRemovedCount;

                var endingPositionIndex = cursorPositionIndex + modifyPositionIndexBy;

                var columnIndex = endingPositionIndex - startOfCurrentRowPositionIndex;
            
                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;
            
                cursorSnapshot.UserCursor.IndexCoordinates = 
                    (indexCoordinates.rowIndex + modifyRowsBy, 
                        columnIndex);
            }
            
            int firstRowIndexToModify;

            if (selectionUpperBoundRowIndex.HasValue)
            {
                firstRowIndexToModify = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
                
                cursorSnapshot.UserCursor.IndexCoordinates = 
                    selectionLowerBoundIndexCoordinates!.Value;
            }
            else if (moveBackwards)
            {
                firstRowIndexToModify = cursorSnapshot.ImmutableCursor.RowIndex - rowsRemovedCount;
            }
            else
            {
                firstRowIndexToModify = cursorSnapshot.ImmutableCursor.RowIndex;
            }
            
            for (int i = firstRowIndexToModify; i < _rowEndingPositions.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositions[i];
            
                _rowEndingPositions[i] = (rowEndingTuple.positionIndex - charactersRemovedCount, rowEndingTuple.rowEndingKind);
            }
            
            var firstTabKeyPositionIndexToModify = _tabKeyPositions
                .FindIndex(x => x >= startingPositionIndexToRemoveInclusive);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (int i = firstTabKeyPositionIndexToModify; i < _tabKeyPositions.Count; i++)
                {
                    _tabKeyPositions[i] -= charactersRemovedCount;
                }
            }
        }
    }

    private void MutateRowEndingKindCount(RowEndingKind rowEndingKind, int changeBy)
    {
        var indexOfRowEndingKindCount = _rowEndingKindCounts
            .FindIndex(x => 
                x.rowEndingKind == rowEndingKind);

        var currentRowEndingKindCount = _rowEndingKindCounts[indexOfRowEndingKindCount]
            .count;
                    
        _rowEndingKindCounts[indexOfRowEndingKindCount] = 
            (rowEndingKind, currentRowEndingKindCount + changeBy);
                    
        CheckRowEndingPositions(false);
    }
    
    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        var existingRowEndings = _rowEndingKindCounts
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndings.Any())
        {
            OnlyRowEndingKind = RowEndingKind.Unset;
            UsingRowEndingKind = RowEndingKind.Linefeed;
        }
        else
        {
            if (existingRowEndings.Length == 1)
            {
                var rowEndingKind = existingRowEndings
                    .Single()
                    .rowEndingKind;

                if (setUsingRowEndingKind)
                    UsingRowEndingKind = rowEndingKind;

                OnlyRowEndingKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                {
                    UsingRowEndingKind = existingRowEndings
                        .MaxBy(x => x.count)
                        .rowEndingKind;
                }
                
                OnlyRowEndingKind = null;
            }
        }
    }
}