using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class TextSelectionGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    
    private string GetTextSelectionStyleCss(int lowerPositionIndexInclusive, int upperPositionIndexExclusive, int rowIndex)
    {
        if (rowIndex >= TextEditorModel.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = TextEditorModel.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorModel.RowEndingPositions[rowIndex];

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

        var topInPixelsInvariantCulture =
            (rowIndex * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture =
            (TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var selectionStartInPixels =
            selectionStartingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionStartingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        var selectionWidthInPixels =
            selectionEndingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionEndingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";

        var fullWidthValue = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;

        if (TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width >
            TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
        }
        
        var fullWidthValueInPixelsInvariantCulture = fullWidthValue
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}