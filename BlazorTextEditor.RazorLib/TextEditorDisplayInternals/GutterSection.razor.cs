using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class GutterSection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;

    private TextEditorRenderStateKey _previousTextEditorRenderStateKey = TextEditorRenderStateKey.Empty;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        var viewModel = TextEditorViewModel;
        
        if (TextEditorViewModel.TextEditorRenderStateKey != _previousTextEditorRenderStateKey)
        {
            _previousTextEditorRenderStateKey = TextEditorViewModel.TextEditorRenderStateKey;
            TextEditorService.SetGutterScrollTopAsync(
                viewModel.GutterElementId,
                viewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollTop);
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private string GetGutterStyleCss(int index)
    {
        var topInPixelsInvariantCulture =
            (index * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var top = $"top: {topInPixelsInvariantCulture}px;";
        
        var heightInPixelsInvariantCulture =
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var height =
            $"height: {heightInPixelsInvariantCulture}px;";

        var mostDigitsInARowLineNumber = TextEditorBase.RowCount
            .ToString()
            .Length;

        var widthInPixels = mostDigitsInARowLineNumber *
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        widthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var widthInPixelsInvariantCulture = widthInPixels.ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var paddingLeftInPixelsInvariantCulture = TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var paddingLeft = $"padding-left: {paddingLeftInPixelsInvariantCulture}px;";
        
        var paddingRightInPixelsInvariantCulture = TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        var paddingRight = $"padding-right: {paddingRightInPixelsInvariantCulture}px;";

        return $"{width} {height} {top} {paddingLeft} {paddingRight}";
    }
    
    private string GetGutterSectionStyleCss()
    {
        var mostDigitsInARowLineNumber = TextEditorBase.RowCount
            .ToString()
            .Length;

        var widthInPixels = mostDigitsInARowLineNumber *
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        widthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;
        
        var widthInPixelsInvariantCulture = widthInPixels.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }

    private IVirtualizationResultWithoutTypeMask GetVirtualizationResult()
    {
        var mostDigitsInARowLineNumber = TextEditorBase.RowCount
            .ToString()
            .Length;

        var widthOfGutterInPixels = mostDigitsInARowLineNumber *
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        widthOfGutterInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var topBoundaryNarrow = TextEditorViewModel.VirtualizationResult.TopVirtualizationBoundary with
        {
            WidthInPixels = widthOfGutterInPixels
        };
        
        var bottomBoundaryNarrow = TextEditorViewModel.VirtualizationResult.BottomVirtualizationBoundary with
        {
            WidthInPixels = widthOfGutterInPixels
        };
        
        return TextEditorViewModel.VirtualizationResult with
        {
            TopVirtualizationBoundary = topBoundaryNarrow,
            BottomVirtualizationBoundary = bottomBoundaryNarrow
        };
    }
}