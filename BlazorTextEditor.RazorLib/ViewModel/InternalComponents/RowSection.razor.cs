using System.Text;
using BlazorALaCarte.Shared.Dimensions;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

public partial class RowSection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public bool GlobalShowNewlines { get; set; }
    [Parameter, EditorRequired]
    public string TabKeyOutput { get; set; } = null!;
    [Parameter, EditorRequired]
    public string SpaceKeyOutput { get; set; } = null!;
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public string HtmlElementId { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    
    public TextEditorCursorDisplay? TextEditorCursorDisplay { get; private set; }
    public MeasureCharacterWidthAndRowHeight? MeasureCharacterWidthAndRowHeightComponent { get; private set; }

    private string GetRowStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        var topInPixelsInvariantCulture =
            (index * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels)
            .ToCssValue();
        
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture =
            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels
                .ToCssValue(); 
        
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.GetValueOrDefault()
            .ToCssValue();
        
        var left = $"left: {virtualizedRowLeftInPixelsInvariantCulture}px;";

        return $"{top} {height} {left}";
    }
    
    private string GetCssClass(byte decorationByte)
    {
        return TextEditorModel.DecorationMapper.Map(decorationByte);
    }
    
    private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }

    private void VirtualizationDisplayItemsProviderFunc(
        VirtualizationRequest virtualizationRequest)
    {
        Task.Run(async () =>
        {
            await TextEditorViewModel.CalculateVirtualizationResultAsync(
                null,
                CancellationToken.None); 
        });
    }
}