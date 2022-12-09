using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Links;

public partial class LinksDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = string.Empty;
}