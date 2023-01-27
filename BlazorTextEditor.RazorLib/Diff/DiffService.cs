namespace BlazorTextEditor.RazorLib.Diff;

public static class DiffService
{
    /// <summary>
    /// This method aims to implement the "An O(ND) Difference Algorithm"
    /// <br/><br/>
    /// Watching https://www.youtube.com/watch?v=9n8jI2267MM
    /// </summary>
    public static void CalculateDiff(
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

                if (weightOfMatchFoundPreviouslyOnThisRow is null)
                {
                    weight = 0;
                    
                    if (beforeCharacterValue is not null &&
                        afterCharacterValue is not null &&
                        beforeCharacterValue.Value == afterCharacterValue.Value)
                    {
                        // TODO: Increase the weight based on amount of preceding matches
                        weight = 1;
                        weightOfMatchFoundPreviouslyOnThisRow = weight;
                    }
                }
                else
                {
                    weight = weightOfMatchFoundPreviouslyOnThisRow.Value;
                }
                
                var diffMatchCell = new DiffMatchCell(
                    rowValue,
                    columnValue,
                    weight);
                
                matchMatrix[beforeIndex, afterIndex] = diffMatchCell;
            }
        }
        
        var z = 2;
    }
}