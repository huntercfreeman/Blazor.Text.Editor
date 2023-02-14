using System.Text;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Diff;

public class DiffResult
{
    private DiffResult(
        string beforeText,
        string afterText,
        DiffMatchCell[,] diffMatrix,
        string longestCommonSubsequence,
        List<TextEditorTextSpan> beforeMatchTextSpans,
        List<TextEditorTextSpan> afterMatchTextSpans)
    {
        BeforeText = beforeText;
        AfterText = afterText;
        DiffMatrix = diffMatrix;
        LongestCommonSubsequence = longestCommonSubsequence;
        BeforeMatchTextSpans = beforeMatchTextSpans;
        AfterMatchTextSpans = afterMatchTextSpans;
    }

    public string BeforeText { get; }
    public string AfterText { get; }
    public DiffMatchCell[,] DiffMatrix { get; }
    public string LongestCommonSubsequence { get; }
    public List<TextEditorTextSpan> BeforeMatchTextSpans { get; }
    public List<TextEditorTextSpan> AfterMatchTextSpans { get; }
    public TextEditorTextSpan TextSpans { get; }

    /// <summary>
    /// This method aims to implement the "An O(ND) Difference Algorithm"
    /// <br/><br/>
    /// Watching https://www.youtube.com/watch?v=9n8jI2267MM
    /// </summary>
    public static DiffResult Calculate(
        string beforeText,
        string afterText)
    {
        // Need to build a square two dimensional array.

        var beforeTextLength = beforeText.Length;
        var afterTextLength = afterText.Length;

        // Envisioning that 'beforeTextLength' represents the rows.
        // And 'afterTextLength' represents the columns.
        var squareSize = Math.Max(beforeTextLength, afterTextLength);

        var matchMatrix = new DiffMatchCell[squareSize, squareSize];

        for (int beforeIndex = 0; beforeIndex < squareSize; beforeIndex++)
        {
            char? beforeCharacterValue = null;

            if (beforeIndex < beforeTextLength)
                beforeCharacterValue = beforeText[beforeIndex];

            int? weightOfMatchFoundPreviouslyOnThisRow = null;

            for (int afterIndex = 0; afterIndex < squareSize; afterIndex++)
            {
                char? afterCharacterValue = null;

                if (afterIndex < afterTextLength)
                    afterCharacterValue = afterText[afterIndex];

                var rowValue = new DiffMatchCellValue(beforeCharacterValue, beforeIndex);

                var columnValue = new DiffMatchCellValue(afterCharacterValue, afterIndex);

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

                var cell = new DiffMatchCell(
                    rowValue,
                    columnValue,
                    weight,
                    isSourceOfRowWeight);

                matchMatrix[beforeIndex, afterIndex] = cell;
            }
        }

        var longestCommonSubsequenceBuilder = new StringBuilder();

        var beforePositionIndicesThatMatchHashSet = new HashSet<int>();
        var afterPositionIndicesThatMatchHashSet = new HashSet<int>();
        
        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            int runningRowIndex = squareSize - 1;
            int runningColumnIndex = squareSize - 1;
            
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

        var diffResult = new DiffResult(
            beforeText,
            afterText,
            matchMatrix,
            longestCommonSubsequenceValue,
            beforeMatchTextSpans,
            afterMatchTextSpans);

        return diffResult;
    }

    private static List<TextEditorTextSpan> GetMatchTextSpans(HashSet<int> positionIndicesThatMatchHashSet)
    {
        var matchTextSpans = new List<TextEditorTextSpan>();

        if (!positionIndicesThatMatchHashSet.Any())
            return matchTextSpans;
        
        var sortedBeforePositionIndicesThatMatch = positionIndicesThatMatchHashSet
            .OrderBy(x => x)
            .ToArray();

        var firstIndex = sortedBeforePositionIndicesThatMatch.First();
            
        var startingIndexInclusive = firstIndex;
        var endingIndexExclusive = firstIndex + 1;
            
        foreach (var index in sortedBeforePositionIndicesThatMatch)
        {
            if (index >= endingIndexExclusive)
            {
                var textSpan = new TextEditorTextSpan(
                    startingIndexInclusive,
                    endingIndexExclusive,
                    (byte)TextEditorDiffDecorationKind.Match);

                matchTextSpans.Add(textSpan);
                    
                startingIndexInclusive = index;
                endingIndexExclusive = index + 1;
            }
            else
            {
                endingIndexExclusive = index + 1;
            }
        }

        return matchTextSpans;
    }
}