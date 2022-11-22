using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class TextSelectionGroup : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    
    private string GetTextSelectionStyleCss(int lowerBound, int upperBound, int rowIndex)
    {
        if (rowIndex >= TextEditor.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = TextEditor.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditor.RowEndingPositions[rowIndex];

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
            $"top: {rowIndex * CharacterWidthAndRowHeight.RowHeightInPixels}px;";
        var height =
            $"height: {CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditor.RowCount
            .ToString()
            .Length;

        var widthOfGutterInPixels = mostDigitsInARowLineNumber *
                                    CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var gutterSizeInPixels =
            widthOfGutterInPixels +
            TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
            TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var selectionStartInPixels =
            selectionStartingColumnIndex *
            CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionStartingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var left = $"left: {gutterSizeInPixels + selectionStartInPixels}px;";

        var selectionWidthInPixels =
            selectionEndingColumnIndex *
            CharacterWidthAndRowHeight.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionEndingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      CharacterWidthAndRowHeight.CharacterWidthInPixels;
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