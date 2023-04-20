using BlazorCommon.RazorLib.Dimensions;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Html;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

public partial class TextSelectionRow : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorOptions GlobalTextEditorOptions { get; set; } = null!;
    [CascadingParameter(Name="ProportionalFontMeasurementsContainerElementId")]
    public string ProportionalFontMeasurementsContainerElementId { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    [Parameter, EditorRequired]
    public string TextSelectionStyleCss { get; set; } = null!;
    [Parameter, EditorRequired]
    public int LowerPositionIndexInclusive { get; set; }
    [Parameter, EditorRequired]
    public int UpperPositionIndexExclusive { get; set; }
    [Parameter, EditorRequired]
    public int RowIndex { get; set; }

    private double _selectionStartingLeftRelativeToParentInPixels;
    private double _selectionWidthInPixels;
    private string _proportionalTextSelectionStyleCss = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!GlobalTextEditorOptions.UseMonospaceOptimizations)
            await GetTextSelectionStyleCssAsync();
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetTextSelectionStyleCssAsync()
    {
        var model = TextEditorModel;
        var viewModel = TextEditorViewModel;
        
        int lowerPositionIndexInclusive = LowerPositionIndexInclusive;
        int upperPositionIndexExclusive = UpperPositionIndexExclusive;
        int rowIndex = RowIndex;
        
        if (rowIndex >= model.RowEndingPositions.Length)
            return;

        bool stateHasChanged = false;
        
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
        
        // _selectionStartingLeftRelativeToParentInPixels
        {
            var selectionStartingCursor = new TextEditorCursor(
                (rowIndex, selectionStartingColumnIndex),
                true);
        
            var textOffsettingCursor = model
                .GetTextOffsettingCursor(selectionStartingCursor)
                .EscapeHtml();
        
            var guid = Guid.NewGuid();
        
            var nextSelectionStartingLeftRelativeToParentInPixels = await JsRuntime.InvokeAsync<double>(
                "blazorTextEditor.calculateProportionalLeftOffset",
                ProportionalFontMeasurementsContainerElementId,
                $"bte_proportional-font-measurement-parent_{viewModel.ViewModelKey.Guid}_selection_{guid}",
                $"bte_proportional-font-measurement-cursor_{viewModel.ViewModelKey.Guid}_selection_{guid}",
                textOffsettingCursor,
                true);
        
            var previousSelectionStartingLeftRelativeToParentInPixels = _selectionStartingLeftRelativeToParentInPixels;
        
            _selectionStartingLeftRelativeToParentInPixels = nextSelectionStartingLeftRelativeToParentInPixels;

            if ((int)nextSelectionStartingLeftRelativeToParentInPixels !=
                (int)previousSelectionStartingLeftRelativeToParentInPixels)
            {
                stateHasChanged = true;
            }
        }

        var selectionStartInPixelsInvariantCulture = _selectionStartingLeftRelativeToParentInPixels
            .ToCssValue();
        
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        // _selectionWidthInPixels
        {
            var selectionEndingCursor = new TextEditorCursor(
                (rowIndex, selectionEndingColumnIndex),
                true);
            
            var textOffsettingCursor = model
                .GetTextOffsettingCursor(selectionEndingCursor)
                .EscapeHtml();
            
            var guid = Guid.NewGuid();
            
            var selectionEndingLeftRelativeToParentInPixels = await JsRuntime.InvokeAsync<double>(
                "blazorTextEditor.calculateProportionalLeftOffset",
                ProportionalFontMeasurementsContainerElementId,
                $"bte_proportional-font-measurement-parent_{viewModel.ViewModelKey.Guid}_selection_{guid}",
                $"bte_proportional-font-measurement-cursor_{viewModel.ViewModelKey.Guid}_selection_{guid}",
                textOffsettingCursor,
                true);
            
            var nextSelectionWidthInPixels =
                selectionEndingLeftRelativeToParentInPixels -
                _selectionStartingLeftRelativeToParentInPixels;
            
            var previousSelectionWidthInPixels = _selectionWidthInPixels;
            
            _selectionWidthInPixels = nextSelectionWidthInPixels;
            
            if ((int)nextSelectionWidthInPixels != (int)previousSelectionWidthInPixels)
                stateHasChanged = true;
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
        
        var selectionWidthInPixelsInvariantCulture = _selectionWidthInPixels
            .ToCssValue();
        
        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        if (stateHasChanged)
            await InvokeAsync(StateHasChanged);

        _proportionalTextSelectionStyleCss = $"{top} {height} {left} {widthCssStyleString}";
    }
}