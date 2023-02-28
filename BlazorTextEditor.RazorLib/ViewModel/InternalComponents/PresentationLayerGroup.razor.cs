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
    
    private string GetPresentationCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        if (rowIndex >= TextEditorModel.RowEndingPositions.Length)
            return string.Empty;

        var startOfRowTuple = TextEditorModel.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorModel.RowEndingPositions[rowIndex];

        var startingColumnIndex = 0;
        var endingColumnIndex =
            endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.positionIndex)
        {
            startingColumnIndex =
                lowerPositionIndexInclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.positionIndex)
        {
            endingColumnIndex =
                upperPositionIndexExclusive - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        var topInPixelsInvariantCulture =
            (rowIndex * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture =
            (TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var startInPixels =
            startingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    startingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey *
                                      tabsOnSameRowBeforeCursor *
                                      TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        var startInPixelsInvariantCulture = startInPixels
            .ToCssValue();
        
        var left = $"left: {startInPixelsInvariantCulture}px;";

        var widthInPixels =
            endingColumnIndex *
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels -
            startInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorModel
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    endingColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey *
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
            .ToCssValue();
        
        var widthInPixelsInvariantCulture = widthInPixels
            .ToCssValue();
        
        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (startingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {startInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{widthInPixelsInvariantCulture}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
    
    private string GetCssClass(
        TextEditorPresentationModel presentationModel,
        byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }
}