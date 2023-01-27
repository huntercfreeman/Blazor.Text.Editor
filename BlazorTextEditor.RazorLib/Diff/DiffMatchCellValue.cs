namespace BlazorTextEditor.RazorLib.Diff;

public class DiffMatchCellValue
{
    public DiffMatchCellValue(
        char? characterValue,
        int positionIndex)
    {
        CharacterValue = characterValue;
        PositionIndex = positionIndex;
    }

    public char? CharacterValue { get; }
    public int PositionIndex { get; }
}