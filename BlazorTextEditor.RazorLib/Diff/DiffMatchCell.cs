namespace BlazorTextEditor.RazorLib.Diff;

public class DiffMatchCell
{
    public DiffMatchCell(
        DiffMatchCellValue valueRow,
        DiffMatchCellValue valueColumn,
        int weight)
    {
        ValueRow = valueRow;
        ValueColumn = valueColumn;
        Weight = weight;
    }

    public DiffMatchCellValue ValueRow { get; }
    public DiffMatchCellValue ValueColumn { get; }
    public int Weight { get; set; }
}