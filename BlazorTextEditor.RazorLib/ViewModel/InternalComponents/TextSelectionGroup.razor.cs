using BlazorCommon.RazorLib.Dimensions;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Html;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

public partial class TextSelectionGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorOptions GlobalTextEditorOptions { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    
    private string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        var model = TextEditorModel;
        var viewModel = TextEditorViewModel;
        
        if (rowIndex >= model.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = model.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = model.RowEndingPositions[rowIndex];

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
            (rowIndex * viewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture =
            (viewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var selectionStartInPixels =
            selectionStartingColumnIndex *
            viewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = model
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionStartingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      viewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels
            .ToCssValue();
        
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        var selectionWidthInPixels =
            selectionEndingColumnIndex *
            viewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = model
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    selectionEndingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      viewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";

        var fullWidthValue = viewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;

        if (viewModel.VirtualizationResult.ElementMeasurementsInPixels.Width >
            viewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = viewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
        }
        
        var fullWidthValueInPixelsInvariantCulture = fullWidthValue
            .ToCssValue();
        
        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels
            .ToCssValue();
        
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