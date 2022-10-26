namespace BlazorTextEditor.RazorLib.Cursor;

public record ImmutableTextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    TextCursorKind TextCursorKind,
    ImmutableTextEditorSelection ImmutableTextEditorSelection)
{
    public ImmutableTextEditorCursor(TextEditorCursor textEditorCursor)
        : this(
            textEditorCursor.IndexCoordinates.rowIndex,
            textEditorCursor.IndexCoordinates.columnIndex,
            textEditorCursor.PreferredColumnIndex,
            textEditorCursor.TextCursorKind,
            new ImmutableTextEditorSelection(textEditorCursor.TextEditorSelection))
    {
    }
}