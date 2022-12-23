using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class BodySection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    
    private VirtualizationDisplay? _virtualizationDisplay;
    
    public TextEditorCursorDisplay? TextEditorCursorDisplay { get; private set; }

    private string GetBodyStyleCss()
    {
        var mostDigitsInARowLineNumber = TextEditorBase.RowCount
            .ToString()
            .Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
                                  TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        gutterWidthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var left = $"left: {gutterWidthInPixels}px;";

        var width = $"width: calc(100% - {gutterWidthInPixels}px);";

        return $"{width} {left}";
    }
}