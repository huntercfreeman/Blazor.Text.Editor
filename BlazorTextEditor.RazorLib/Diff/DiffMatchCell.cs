namespace BlazorTextEditor.RazorLib.Diff;

public class DiffMatchCell
{
    public DiffMatchCell(
        DiffMatchCellValue valueRow,
        DiffMatchCellValue valueColumn,
        int weight,
        bool isSourceOfRowWeight)
    {
        ValueRow = valueRow;
        ValueColumn = valueColumn;
        Weight = weight;
        IsSourceOfRowWeight = isSourceOfRowWeight;
    }

    public DiffMatchCellValue ValueRow { get; }
    public DiffMatchCellValue ValueColumn { get; }
    public int Weight { get; set; }
    public bool IsSourceOfRowWeight { get; }
}