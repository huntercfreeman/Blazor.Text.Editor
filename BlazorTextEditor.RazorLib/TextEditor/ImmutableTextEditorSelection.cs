namespace BlazorTextEditor.RazorLib.TextEditor;

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

    public bool HasSelectedText()
    {
        if (AnchorPositionIndex.HasValue &&
            AnchorPositionIndex.Value !=
            EndingPositionIndex)
        {
            return true;
        }

        return false;
    }
}