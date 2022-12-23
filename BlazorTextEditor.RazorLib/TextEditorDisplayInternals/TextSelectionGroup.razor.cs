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
    
    private string GetTextSelectionStyleCss(int lowerBound, int upperBound, int rowIndex)
    {
        if (rowIndex >= TextEditorBase.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = TextEditorBase.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorBase.RowEndingPositions[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex =
            endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerBound > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex =
                lowerBound - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (upperBound < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex =
                upperBound - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        var top =
            $"top: {rowIndex * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";
        var height =
            $"height: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorBase.RowCount
            .ToString()
            .Length;

        var widthOfGutterInPixels = mostDigitsInARowLineNumber *
                                    TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var gutterSizeInPixels =
            widthOfGutterInPixels +
            TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
            TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

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

        var left = $"left: {gutterSizeInPixels + selectionStartInPixels}px;";

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
            widthCssStyleString += "100%";
        else if (selectionStartingColumnIndex != 0 &&
                 upperBound > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc(100% - {selectionStartInPixels}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixels}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}