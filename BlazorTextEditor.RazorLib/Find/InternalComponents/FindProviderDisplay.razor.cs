using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Find.InternalComponents;

public partial class FindProviderDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ITextEditorFindProvider TextEditorFindProvider { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsActive { get; set; }

    private string IsActiveCssClassString => IsActive
        ? "bcrl_active"
        : "";
}