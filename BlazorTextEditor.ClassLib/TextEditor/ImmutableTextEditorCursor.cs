namespace BlazorTextEditor.ClassLib.TextEditor;

public record ImmutableTextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    TextCursorKind TextCursorKind)
{
    public ImmutableTextEditorCursor(TextEditorCursor textEditorCursor) 
        : this(
            textEditorCursor.IndexCoordinates.rowIndex,
            textEditorCursor.IndexCoordinates.columnIndex,
            textEditorCursor.PreferredColumnIndex,
            textEditorCursor.TextCursorKind)
    {
    }
}