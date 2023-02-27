namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorDiffMatchCell
{
    public TextEditorDiffMatchCell(
        TextEditorDiffMatchCellValue valueRow,
        TextEditorDiffMatchCellValue valueColumn,
        int weight,
        bool isSourceOfRowWeight)
    {
        ValueRow = valueRow;
        ValueColumn = valueColumn;
        Weight = weight;
        IsSourceOfRowWeight = isSourceOfRowWeight;
    }

    public TextEditorDiffMatchCellValue ValueRow { get; }
    public TextEditorDiffMatchCellValue ValueColumn { get; }
    public int Weight { get; set; }
    public bool IsSourceOfRowWeight { get; }
}