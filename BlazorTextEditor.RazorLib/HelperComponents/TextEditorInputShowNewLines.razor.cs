using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputShowNewLines : FluxorComponent
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public string LabelElementCssClassString { get; set; } = string.Empty;

    public bool GlobalShowNewlines
    {
        get => TextEditorService.GlobalShowNewlines;
        set => TextEditorService.SetShowNewlines(value);
    }
}