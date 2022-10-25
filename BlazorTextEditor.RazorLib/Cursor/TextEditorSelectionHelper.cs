using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Cursor;

public static class TextEditorSelectionHelper
{
    public static bool HasSelectedText(TextEditorSelection textEditorSelection)
    {
        return HasSelectedText(
            textEditorSelection.AnchorPositionIndex, 
            textEditorSelection.EndingPositionIndex);
    }
    
    public static bool HasSelectedText(ImmutableTextEditorSelection immutableTextEditorSelection)
    {
        return HasSelectedText(
            immutableTextEditorSelection.AnchorPositionIndex, 
            immutableTextEditorSelection.EndingPositionIndex);
    }
    
    public static bool HasSelectedText(int? anchorPositionIndex, int endingPositionIndex)
    {
        if (anchorPositionIndex.HasValue &&
            anchorPositionIndex.Value !=
            endingPositionIndex)
        {
            return true;
        }

        return false;
    }
    
    public static string? GetSelectedText(
        TextEditorSelection textEditorSelection, 
        TextEditorBase textEditorBase)
    {
        return GetSelectedText(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex,
            textEditorBase);
    }
    
    public static string? GetSelectedText(
        ImmutableTextEditorSelection immutableTextEditorSelection,
        TextEditorBase textEditorBase)
    {
        return GetSelectedText(
            immutableTextEditorSelection.AnchorPositionIndex,
            immutableTextEditorSelection.EndingPositionIndex,
            textEditorBase);
    }
    
    public static string? GetSelectedText(
        int? anchorPositionIndex, 
        int endingPositionIndex, 
        TextEditorBase textEditorBase)
    {
        if (HasSelectedText(
                anchorPositionIndex,
                endingPositionIndex))
        {
            var selectionBounds = GetSelectionBounds(
                anchorPositionIndex,
                endingPositionIndex);
            
            var lowerBound = anchorPositionIndex.Value;
            var upperBound = endingPositionIndex;

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

    public static (int lowerBound, int upperBound) GetSelectionBounds(
        TextEditorSelection textEditorSelection)
    {
        return GetSelectionBounds(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex);
    }
    
    public static (int lowerBound, int upperBound) GetSelectionBounds(
        ImmutableTextEditorSelection immutableTextEditorSelection)
    {
        return GetSelectionBounds(
            immutableTextEditorSelection.AnchorPositionIndex,
            immutableTextEditorSelection.EndingPositionIndex);
    }
    
    public static (int lowerBound, int upperBound) GetSelectionBounds(
        int? anchorPositionIndex,
        int endingPositionIndex)
    {
        if (anchorPositionIndex is null)
        {
            throw new ApplicationException(
                $"{nameof(anchorPositionIndex)} was null.");
        }
        
        var lowerBound = anchorPositionIndex.Value;
        var upperBound = endingPositionIndex;

        if (lowerBound > upperBound)
        {
            // Swap the values around
            (lowerBound, upperBound) = (upperBound, lowerBound);
        }

        return (lowerBound, upperBound);
    }
}