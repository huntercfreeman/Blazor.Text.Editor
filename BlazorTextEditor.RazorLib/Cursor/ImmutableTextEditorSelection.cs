using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.MoveThese;

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
    
    // TODO: Don't keep this as is it is duplicate code from TextEditorSelection.cs
    public string? GetSelectedText(TextEditorBase textEditorBase)
    {
        if (AnchorPositionIndex.HasValue &&
            AnchorPositionIndex.Value !=
            EndingPositionIndex)
        {
            var lowerBound = AnchorPositionIndex.Value;
            var upperBound = EndingPositionIndex;

            if (lowerBound > upperBound)
            {
                (lowerBound, upperBound) = (upperBound, lowerBound);
            }

            var result = textEditorBase.GetTextRange(lowerBound,
                upperBound - lowerBound);

            return result.Length != 0
                ? result
                : null;
        }

        return null;
    }
}