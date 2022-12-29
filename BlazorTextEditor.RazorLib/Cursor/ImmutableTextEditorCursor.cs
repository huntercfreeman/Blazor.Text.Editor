namespace BlazorTextEditor.RazorLib.Cursor;

public record ImmutableTextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    ImmutableTextEditorSelection ImmutableTextEditorSelection)
{
    public ImmutableTextEditorCursor(TextEditorCursor textEditorCursor)
        : this(
            textEditorCursor.IndexCoordinates.rowIndex,
            textEditorCursor.IndexCoordinates.columnIndex,
            textEditorCursor.PreferredColumnIndex,
            new ImmutableTextEditorSelection(textEditorCursor.TextEditorSelection))
    {
    }
}