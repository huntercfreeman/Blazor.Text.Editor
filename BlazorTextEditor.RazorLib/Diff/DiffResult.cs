using System.Text;

namespace BlazorTextEditor.RazorLib.Diff;

public class DiffResult
{
    private DiffResult(
        string beforeText,
        string afterText,
        DiffMatchCell[,] diffMatrix,
        string longestCommonSubsequence)
    {
        BeforeText = beforeText;
        AfterText = afterText;
        DiffMatrix = diffMatrix;
        LongestCommonSubsequence = longestCommonSubsequence;
    }

    public string BeforeText { get; }
    public string AfterText { get; }
    public DiffMatchCell[,] DiffMatrix { get; }
    public string LongestCommonSubsequence { get; }

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
        
        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            int runningRowIndex = squareSize;
            int runningColumnIndex = squareSize;

            var isFirstLoop = true;
            var previousRowWeight = 0;
            
            while (runningRowIndex != 0 && runningColumnIndex != 0)
            {
                var cell = matchMatrix[runningRowIndex - 1, runningColumnIndex - 1];

                if ((isFirstLoop || cell.Weight == previousRowWeight - 1) &&
                    cell.IsSourceOfRowWeight)
                {
                    longestCommonSubsequenceBuilder
                        .Append(cell.ValueColumn.CharacterValue);
                    
                    previousRowWeight = cell.Weight + 1;
                    runningRowIndex--;
                    continue;
                }

                runningColumnIndex--;
            }
        }

        var longestCommonSubsequenceValue = new string(longestCommonSubsequenceBuilder
            .ToString()
            .Reverse()
            .ToArray());
        
        var diffResult = new DiffResult(
            beforeText,
            afterText,
            matchMatrix,
            longestCommonSubsequenceValue);

        return diffResult;
    }
}