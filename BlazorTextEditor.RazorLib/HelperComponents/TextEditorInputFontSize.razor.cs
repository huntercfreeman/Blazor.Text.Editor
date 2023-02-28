using BlazorTextEditor.RazorLib.Store.Model;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputFontSize : FluxorComponent
{
    [Inject]
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter(Name="InputElementCssClass")]
    public string CascadingInputElementCssClass { get; set; } = string.Empty;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;

    private const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    
    private int TextEditorFontSize
    {
        get => TextEditorService.GlobalOptionsWrap.Value.Options.CommonOptions.FontSizeInPixels 
               ?? MINIMUM_FONT_SIZE_IN_PIXELS;
        set
        {
            if (value < MINIMUM_FONT_SIZE_IN_PIXELS)
                value = MINIMUM_FONT_SIZE_IN_PIXELS;
            
            TextEditorService.GlobalOptionsSetFontSize(value);
        }
    }
}