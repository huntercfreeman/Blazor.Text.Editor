using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorDiffResult
{
    private TextEditorDiffResult(
        string beforeText,
        string afterText,
        TextEditorDiffMatchCell[,] diffMatrix,
        string longestCommonSubsequence,
        ImmutableList<TextEditorTextSpan> beforeLongestCommonSubsequenceTextSpans,
        ImmutableList<TextEditorTextSpan> afterLongestCommonSubsequenceTextSpans)
    {
        BeforeText = beforeText;
        AfterText = afterText;
        DiffMatrix = diffMatrix;
        LongestCommonSubsequence = longestCommonSubsequence;
        BeforeLongestCommonSubsequenceTextSpans = beforeLongestCommonSubsequenceTextSpans;
        AfterLongestCommonSubsequenceTextSpans = afterLongestCommonSubsequenceTextSpans;
    }

    public string BeforeText { get; }
    public string AfterText { get; }
    public TextEditorDiffMatchCell[,] DiffMatrix { get; }
    public string LongestCommonSubsequence { get; }
    public ImmutableList<TextEditorTextSpan> BeforeLongestCommonSubsequenceTextSpans { get; }
    public ImmutableList<TextEditorTextSpan> AfterLongestCommonSubsequenceTextSpans { get; }
    public TextEditorTextSpan TextSpans { get; }

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

        var matchMatrix = new TextEditorDiffMatchCell[squaredSize, squaredSize];

        for (int beforeIndex = 0; beforeIndex < beforeTextLength; beforeIndex++)
        {
            char? beforeCharacterValue = beforeText[beforeIndex];

            for (int afterIndex = 0; afterIndex < afterTextLength; afterIndex++)
            {
                char? afterCharacterValue = afterText[afterIndex];

                int weight;
                bool isSourceOfRowWeight = false;

                if (weightOfMatchFoundPreviouslyOnThisRow is null)
                {
                    weight = 0;

                    if (beforeCharacterValue is not null &&
                        afterCharacterValue is not null &&
                        beforeCharacterValue.Value == afterCharacterValue.Value)
                    {
                        isSourceOfRowWeight = true;

                        int runningRowIndex = beforeIndex;
                        int runningColumnIndex = afterIndex;

                        while (runningRowIndex != 0 && runningColumnIndex != 0)
                        {
                            var readWeight = matchMatrix[runningRowIndex - 1, runningColumnIndex - 1].Weight;

                            if (readWeight != 0)
                            {
                                weight = readWeight + 1;
                                break;
                            }

                            // Mutate loop variables
                            {
                                runningRowIndex--;
                            }
                        }

                        if (weight == 0)
                            weight = 1;

                        weightOfMatchFoundPreviouslyOnThisRow = weight;
                    }
                }
                else
                {
                    weight = weightOfMatchFoundPreviouslyOnThisRow.Value;
                }

                var cell = new TextEditorDiffMatchCell(
                    rowValue,
                    columnValue,
                    weight,
                    isSourceOfRowWeight);

                matchMatrix[beforeIndex, afterIndex] = cell;
            }
            
            if (afterTextLength < beforeTextLength)
            {
                // TODO: This if block for setting fabricated columns.
                
                // This if block sets the cells in the fabricated column
                // in order to create a square matrix
            }
        }

        if (beforeTextLength < afterTextLength)
        {
            // TODO: This if block for setting fabricated rows.
            
            // This if block sets the cells in the fabricated row
            // in order to create a square matrix
        }

        var longestCommonSubsequenceBuilder = new StringBuilder();

        var beforePositionIndicesThatMatchHashSet = new HashSet<int>();
        var afterPositionIndicesThatMatchHashSet = new HashSet<int>();
        
        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            int runningRowIndex = squaredSize - 1;
            int runningColumnIndex = squaredSize - 1;
            
            while (runningRowIndex != -1 && runningColumnIndex != -1)
            {
                var restoreColumnIndex = runningColumnIndex;
                
                var cell = matchMatrix[runningRowIndex, runningColumnIndex];

                if (cell.IsSourceOfRowWeight)
                {
                    longestCommonSubsequenceBuilder
                        .Append(cell.ValueColumn.CharacterValue);

                    beforePositionIndicesThatMatchHashSet.Add(cell.ValueRow.PositionIndex);
                    afterPositionIndicesThatMatchHashSet.Add(cell.ValueColumn.PositionIndex);
                    
                    runningColumnIndex--;
                    runningRowIndex--;
                    
                    continue;
                }

                if (runningColumnIndex == 0)
                {
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

        var beforeMatchTextSpans = GetMatchTextSpans(beforePositionIndicesThatMatchHashSet);
        var afterMatchTextSpans = GetMatchTextSpans(afterPositionIndicesThatMatchHashSet);

        var diffResult = new TextEditorDiffResult(
            beforeText,
            afterText,
            matchMatrix,
            longestCommonSubsequenceValue,
            beforeMatchTextSpans.ToImmutableList(),
            afterMatchTextSpans.ToImmutableList());

        return diffResult;
    }

    private static List<TextEditorTextSpan> GetMatchTextSpans(HashSet<int> positionIndicesThatMatchHashSet)
    {
        var matchTextSpans = new List<TextEditorTextSpan>();

        if (!positionIndicesThatMatchHashSet.Any())
            return matchTextSpans;
        
        var sortedPositionIndicesThatMatch = positionIndicesThatMatchHashSet
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
                    (byte)TextEditorDiffDecorationKind.Match);

                matchTextSpans.Add(textSpan);
                    
                startingIndexInclusive = index;
                endingIndexExclusive = index + 1;
            }
        }

        return matchTextSpans;
    }
}