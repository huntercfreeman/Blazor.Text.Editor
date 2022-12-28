using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputCursorWidth : FluxorComponent
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;

    private const double MINIMUM_CURSOR_SIZE_IN_PIXELS = 1;
    
    private double TextEditorCursorWidth
    {
        get => TextEditorStatesWrap.Value.GlobalTextEditorOptions.CursorWidthInPixels 
               ?? MINIMUM_CURSOR_SIZE_IN_PIXELS;
        set
        {
            if (value < MINIMUM_CURSOR_SIZE_IN_PIXELS)
                value = MINIMUM_CURSOR_SIZE_IN_PIXELS;
            
            TextEditorService.SetCursorWidth(value);
        }
    }
}