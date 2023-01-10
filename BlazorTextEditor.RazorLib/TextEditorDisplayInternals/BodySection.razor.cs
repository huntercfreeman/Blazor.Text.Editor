using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class BodySection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public string HtmlElementId { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    
    private RowSection? _rowSection;

    public TextEditorCursorDisplay? TextEditorCursorDisplay => _rowSection?.TextEditorCursorDisplay;
    public MeasureCharacterWidthAndRowHeight? MeasureCharacterWidthAndRowHeightComponent => _rowSection?.MeasureCharacterWidthAndRowHeightComponent;

    private string GetBodyStyleCss()
    {
        var mostDigitsInARowLineNumber = TextEditorModel.RowCount
            .ToString()
            .Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
                                  TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var gutterWidthInPixelsInvariantCulture = gutterWidthInPixels
            .ToString(System.Globalization.CultureInfo.InvariantCulture); 

        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";

        return $"{width} {left}";
    }
}