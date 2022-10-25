namespace BlazorTextEditor.RazorLib.Cursor;

public record ImmutableTextEditorSelection(
    int? AnchorPositionIndex,
    int EndingPositionIndex)
{
    public ImmutableTextEditorSelection(TextEditorSelection textEditorSelection)
        : this(
            textEditorSelection.AnchorPositionIndex, 
            textEditorSelection.EndingPositionIndex)
    {
    }
}