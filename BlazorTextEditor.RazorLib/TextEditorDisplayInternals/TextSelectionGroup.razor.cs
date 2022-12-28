using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class TextSelectionGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    
    private string GetTextSelectionStyleCss(int lowerPositionIndexInclusive, int upperPositionIndexExclusive, int rowIndex)
    {
        if (rowIndex >= TextEditorBase.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = TextEditorBase.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorBase.RowEndingPositions[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex =
            endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex =
                lowerPositionIndexInclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex =
                upperPositionIndexExclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        var top =
            $"top: {rowIndex * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";
        var height =
            $"height: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var selectionStartInPixels =
            selectionStartingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorBase
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionStartingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var left = $"left: {selectionStartInPixels}px;";

        var selectionWidthInPixels =
            selectionEndingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorBase
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionEndingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth}px - {selectionStartInPixels}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixels}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}