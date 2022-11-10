using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputShowWhitespace : FluxorComponent
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public string InputElementCssClassString { get; set; } = string.Empty;

    public bool GlobalShowWhitespace
    {
        get => TextEditorService.GlobalShowWhitespace;
        set => TextEditorService.SetShowWhitespace(value);
    }
}