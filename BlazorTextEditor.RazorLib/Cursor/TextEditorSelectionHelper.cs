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
            return true;

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

            var result = textEditorBase.GetTextRange(selectionBounds.lowerPositionIndexInclusive,
                selectionBounds.upperPositionIndexExclusive - selectionBounds.lowerPositionIndexInclusive);

            return result.Length != 0
                ? result
                : null;
        }

        return null;
    }

    public static TextEditorCursor SelectLinesRange(
        TextEditorBase textEditorBase,
        int startingRowIndex, 
        int count)
    {
        var startingPositionIndexInclusive = textEditorBase.GetPositionIndex(
            startingRowIndex,
            0);

        var lastRowIndexExclusive = startingRowIndex + count;
        
        var endingPositionIndexExclusive = textEditorBase.GetPositionIndex(
            lastRowIndexExclusive,
            0);

        var textEditorCursor = new TextEditorCursor(
            (startingRowIndex, 0),
            false);

        textEditorCursor.TextEditorSelection.AnchorPositionIndex = startingPositionIndexInclusive;
        textEditorCursor.TextEditorSelection.EndingPositionIndex = endingPositionIndexExclusive;

        return textEditorCursor;
    }
    
    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        TextEditorSelection textEditorSelection)
    {
        return GetSelectionBounds(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex);
    }

    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        ImmutableTextEditorSelection immutableTextEditorSelection)
    {
        return GetSelectionBounds(
            immutableTextEditorSelection.AnchorPositionIndex,
            immutableTextEditorSelection.EndingPositionIndex);
    }

    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        int? anchorPositionIndex,
        int endingPositionIndex)
    {
        if (anchorPositionIndex is null)
        {
            throw new ApplicationException(
                $"{nameof(anchorPositionIndex)} was null.");
        }

        var lowerPositionIndexInclusive = anchorPositionIndex.Value;
        var upperPositionIndexExclusive = endingPositionIndex;

        if (lowerPositionIndexInclusive > upperPositionIndexExclusive)
            // Swap the values around
            (lowerPositionIndexInclusive, upperPositionIndexExclusive) = (upperPositionIndexExclusive, lowerPositionIndexInclusive);

        return (lowerPositionIndexInclusive, upperPositionIndexExclusive);
    }
    
    public static (int lowerRowIndexInclusive, int upperRowIndexExclusive) ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
        TextEditorBase textEditorBase,
        (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) positionIndexBounds)
    {
        var firstRowToSelectDataInclusive =
            textEditorBase
                .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                    positionIndexBounds.lowerPositionIndexInclusive).rowIndex;

        var lastRowToSelectDataExclusive =
            textEditorBase
                .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                    positionIndexBounds.upperPositionIndexExclusive).rowIndex +
            1;
        
        return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
    }
}