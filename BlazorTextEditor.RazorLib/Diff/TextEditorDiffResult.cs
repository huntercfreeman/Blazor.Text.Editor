using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorDiffResult
{
    private TextEditorDiffResult(
        string beforeText,
        string afterText,
        TextEditorDiffCell[,] diffMatrix,
        (int sourceWeight, int beforeIndex, int afterIndex) highestSourceWeightTuple,
        string longestCommonSubsequence,
        ImmutableList<TextEditorTextSpan> beforeLongestCommonSubsequenceTextSpans,
        ImmutableList<TextEditorTextSpan> afterLongestCommonSubsequenceTextSpans)
    {
        BeforeText = beforeText;
        AfterText = afterText;
        DiffMatrix = diffMatrix;
        HighestSourceWeightTuple = highestSourceWeightTuple;
        LongestCommonSubsequence = longestCommonSubsequence;
        BeforeLongestCommonSubsequenceTextSpans = beforeLongestCommonSubsequenceTextSpans;
        AfterLongestCommonSubsequenceTextSpans = afterLongestCommonSubsequenceTextSpans;
    }

    public string BeforeText { get; }
    public string AfterText { get; }
    public TextEditorDiffCell[,] DiffMatrix { get; }
    public (int sourceWeight, int beforeIndex, int afterIndex) HighestSourceWeightTuple { get; }
    public string LongestCommonSubsequence { get; }
    public ImmutableList<TextEditorTextSpan> BeforeLongestCommonSubsequenceTextSpans { get; }
    public ImmutableList<TextEditorTextSpan> AfterLongestCommonSubsequenceTextSpans { get; }

    /// <summary>
    /// This method aims to implement the "An O(ND) Difference Algorithm"
    /// <br/><br/>
    /// Watching https://www.youtube.com/watch?v=9n8jI2267MM
    /// </summary>
    public static TextEditorDiffResult Calculate(
        string beforeText,
        string afterText)
    {
        // Need to build a square two dimensional array.
        //
        // Envisioning that 'beforeText' represents the rows
        // and 'afterText' represents the columns.
        var beforeTextLength = beforeText.Length;
        var afterTextLength = afterText.Length;
        
        var squaredSize = Math.Max(beforeTextLength, afterTextLength);

        var diffMatrix = new TextEditorDiffCell[squaredSize, squaredSize];
        
        (int sourceWeight, int beforeIndex, int afterIndex) highestSourceWeightTuple = (-1, -1, -1);

        for (int beforeIndex = 0; beforeIndex < beforeTextLength; beforeIndex++)
        {
            char beforeCharValue = beforeText[beforeIndex];

            for (int afterIndex = 0; afterIndex < afterTextLength; afterIndex++)
            {
                char afterCharValue = afterText[afterIndex];

                var cellSourceWeight = GetCellSourceWeight(
                    diffMatrix,
                    beforeIndex,
                    afterIndex);
                
                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    beforeIndex,
                    afterIndex);

                if (beforeCharValue == afterCharValue &&
                    cellSourceWeight >= rowLargestWeightPriorToCurrentColumn)
                {
                    diffMatrix[beforeIndex, afterIndex] = new TextEditorDiffCell(
                        beforeCharValue,
                        afterCharValue,
                        cellSourceWeight,
                        true);
                    
                    if (cellSourceWeight > highestSourceWeightTuple.sourceWeight)
                        highestSourceWeightTuple = (cellSourceWeight, beforeIndex, afterIndex);
                }
                else
                {
                    diffMatrix[beforeIndex, afterIndex] = new TextEditorDiffCell(
                        beforeCharValue,
                        afterCharValue,
                        rowLargestWeightPriorToCurrentColumn,
                        false);
                }
            }
            
            for (
                int fabricatedAfterIndex = afterTextLength;
                fabricatedAfterIndex < squaredSize;
                fabricatedAfterIndex++)
            {
                // This for loop sets the cells in the fabricated column
                // in order to create a square matrix
                // where afterTextLength < beforeTextLength
                
                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    beforeIndex,
                    fabricatedAfterIndex);
                
                diffMatrix[beforeIndex, fabricatedAfterIndex] = new TextEditorDiffCell(
                    beforeCharValue,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        for (
            int fabricatedBeforeIndex = beforeTextLength;
            fabricatedBeforeIndex < squaredSize;
            fabricatedBeforeIndex++)
        {
            // This for loop sets the cells in the fabricated row
            // in order to create a square matrix
            // where beforeTextLength < afterTextLength
            //
            // TODO: This logic should to be removed. Instead of looking at this algorithm from the perspective of 'before' and 'after' text. It might be best to have the perspective be 'vertical' and 'horizontal' text. Then the 'vertical' text is equal to the longer of the two string inputs. By having the row count defined by the longer of the two strings you only would ever fabricate columns. This would allow for a lot of code to be removed.
            
            for (
                int fabricatedAfterIndex = 0;
                fabricatedAfterIndex < squaredSize;
                fabricatedAfterIndex++)
            {
                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    fabricatedBeforeIndex,
                    fabricatedAfterIndex);
                
                diffMatrix[fabricatedBeforeIndex, fabricatedAfterIndex] = new TextEditorDiffCell(
                    null,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        var longestCommonSubsequenceBuilder = new StringBuilder();

        var beforePositionIndicesOfLongestCommonSubsequenceHashSet = new HashSet<int>();
        var afterPositionIndicesOfLongestCommonSubsequenceHashSet = new HashSet<int>();
        
        var afterPositionIndicesOfInsertionHashSet = new HashSet<int>();
        
        var beforePositionIndicesOfDeletionHashSet = new HashSet<int>();
        
        var beforePositionIndicesOfModificationHashSet = new HashSet<int>();
        var afterPositionIndicesOfModificationHashSet = new HashSet<int>();
        
        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            int runningRowIndex = highestSourceWeightTuple.beforeIndex;
            int runningColumnIndex = highestSourceWeightTuple.afterIndex;

            bool decoratingRemainingColumns = false;

            var restoreColumnIndex = runningColumnIndex;
            
            while (runningRowIndex != -1 && runningColumnIndex != -1)
            {
                if (!decoratingRemainingColumns)
                    restoreColumnIndex = runningColumnIndex;
                
                var cell = diffMatrix[runningRowIndex, runningColumnIndex];

                if (cell.IsSourceOfRowWeight && 
                    !decoratingRemainingColumns)
                {
                    if (cell.BeforeCharValue != cell.AfterCharValue)
                    {
                        throw new ApplicationException(
                            $"The {nameof(cell.BeforeCharValue)}:'{cell.BeforeCharValue}' was not equal to the {nameof(cell.AfterCharValue)}:'{cell.AfterCharValue}'");
                    }
                    
                    longestCommonSubsequenceBuilder
                        .Append(cell.BeforeCharValue);

                    // Decoration logic for longest common subsequence
                    {
                        beforePositionIndicesOfLongestCommonSubsequenceHashSet.Add(runningRowIndex);
                        afterPositionIndicesOfLongestCommonSubsequenceHashSet.Add(runningColumnIndex);
                    }

                    decoratingRemainingColumns = true;
                    restoreColumnIndex = runningColumnIndex - 1;
                }
                else
                {
                    // Decoration logic
                    {
                        if (afterTextLength > beforeTextLength &&
                            runningColumnIndex > beforeTextLength - 1)
                        {
                            // Insertion
                            afterPositionIndicesOfInsertionHashSet.Add(runningRowIndex);
                        }
                        else if (beforeTextLength > afterTextLength &&
                                 runningRowIndex >= afterTextLength)
                        {
                            // Deletion
                            beforePositionIndicesOfDeletionHashSet.Add(runningColumnIndex);
                        }
                        // TODO: Else if for modification is not working.
                        //
                        // else if (cell.BeforeCharValue != cell.AfterCharValue)
                        // {
                        //     // Modification
                        //     beforePositionIndicesOfModificationHashSet.Add(runningRowIndex);
                        //     afterPositionIndicesOfModificationHashSet.Add(runningColumnIndex);
                        // }
                    }
                }

                if (runningColumnIndex == 0)
                {
                    decoratingRemainingColumns = false;
                    
                    runningColumnIndex = restoreColumnIndex;
                    runningRowIndex--;
                }
                else
                {
                    runningColumnIndex--;
                }
            }
        }

        var longestCommonSubsequenceValue = new string(longestCommonSubsequenceBuilder
            .ToString()
            .Reverse()
            .ToArray());
        
        var beforeTextSpans = new List<TextEditorTextSpan>();
        var afterTextSpans = new List<TextEditorTextSpan>();
        
        // Decoration logic
        {
            // Longest common subsequence
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforePositionIndicesOfLongestCommonSubsequenceHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));
                
                afterTextSpans.AddRange(GetTextSpans(
                    afterPositionIndicesOfLongestCommonSubsequenceHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));
            }
            
            // Insertion
            {
                afterTextSpans.AddRange(GetTextSpans(
                    afterPositionIndicesOfInsertionHashSet,
                    (byte)TextEditorDiffDecorationKind.Insertion));
            }
            
            // Deletion
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforePositionIndicesOfDeletionHashSet,
                    (byte)TextEditorDiffDecorationKind.Deletion));
            }
            
            // Modification
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforePositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));
                
                afterTextSpans.AddRange(GetTextSpans(
                    afterPositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));
            }
        }

        var diffResult = new TextEditorDiffResult(
            beforeText,
            afterText,
            diffMatrix,
            highestSourceWeightTuple,
            longestCommonSubsequenceValue,
            beforeTextSpans.ToImmutableList(),
            afterTextSpans.ToImmutableList());

        return diffResult;
    }
    
    private static int GetCellSourceWeight(
        TextEditorDiffCell[,] diffMatrix,
        int beforeIndex,
        int afterIndex)
    {
        if (beforeIndex > 0 &&
            afterIndex > 0)
        {
            return diffMatrix[beforeIndex - 1, afterIndex - 1]
                       .Weight +
                   1;
        }

        return 1;
    }
    
    private static int GetRowLargestWeightPriorToCurrentColumn(
        TextEditorDiffCell[,] diffMatrix,
        int beforeIndex,
        int afterIndex)
    {
        if (afterIndex > 0)
        {
            return diffMatrix[beforeIndex, afterIndex - 1]
                .Weight;
        }

        return 0;
    }

    private static List<TextEditorTextSpan> GetTextSpans(
        HashSet<int> positionIndicesHashSet,
        byte decorationByte)
    {
        var matchTextSpans = new List<TextEditorTextSpan>();

        if (!positionIndicesHashSet.Any())
            return matchTextSpans;
        
        var sortedPositionIndicesThatMatch = positionIndicesHashSet
            .OrderBy(x => x)
            .ToList();

        // The foreach loop coalesces contiguous indices into a single TextEditorTextSpan
        // and the logic that does the coalescing will not write out the final index without this.
        sortedPositionIndicesThatMatch.Add(int.MaxValue);

        var startingIndexInclusive = sortedPositionIndicesThatMatch.First();
        var endingIndexExclusive = startingIndexInclusive + 1;

        var skipFirstIndex = sortedPositionIndicesThatMatch
            .Skip(1)
            .ToArray();
        
        foreach (var index in skipFirstIndex)
        {
            if (index == endingIndexExclusive)
            {
                endingIndexExclusive++;
            }
            else
            {
                var textSpan = new TextEditorTextSpan(
                    startingIndexInclusive,
                    endingIndexExclusive,
                    decorationByte);

                matchTextSpans.Add(textSpan);
                    
                startingIndexInclusive = index;
                endingIndexExclusive = index + 1;
            }
        }

        return matchTextSpans;
    }
}