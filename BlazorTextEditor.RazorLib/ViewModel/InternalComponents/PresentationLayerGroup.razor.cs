using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Model;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

public partial class PresentationLayerGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public ImmutableList<TextEditorPresentationModel> TextEditorPresentations { get; set; } = ImmutableList<TextEditorPresentationModel>.Empty;
    
    private string GetCssStyleString(
        int startingPositionIndexInclusive,
        int endingPositionIndexExclusive,
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel)
    {
        var startingRowInformation = textEditorModel.FindRowInformation(
            startingPositionIndexInclusive);
        
        if (startingRowInformation.rowIndex >= textEditorModel.RowEndingPositions.Length)
            return string.Empty;
        
        var startOfRowTuple = textEditorModel.GetStartOfRowTuple(startingRowInformation.rowIndex);
        var endOfRowTuple = textEditorModel.RowEndingPositions[startingRowInformation.rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex =
            endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (startingPositionIndexInclusive > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex =
                startingPositionIndexInclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (endingPositionIndexExclusive < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex =
                endingPositionIndexExclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        var topInPixelsInvariantCulture =
            (startingRowInformation.rowIndex * textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture =
            (textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var selectionStartInPixels =
            selectionStartingColumnIndex *
            textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = textEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    startingRowInformation.rowIndex,
                    selectionStartingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels
            .ToCssValue();
        
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        var selectionWidthInPixels =
            selectionEndingColumnIndex *
            textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = textEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    startingRowInformation.rowIndex,
                    selectionEndingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";

        var fullWidthValue = textEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;

        if (textEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width >
            textEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = textEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
        }
        
        var fullWidthValueInPixelsInvariantCulture = fullWidthValue
            .ToCssValue();
        
        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels
            .ToCssValue();
        
        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 endingPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}