using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Icons;

public class IconBase : ComponentBase
{
    [CascadingParameter(Name="BlazorTextEditorIconWidth")]
    public int WidthInPixels { get; set; } = 16;
    [CascadingParameter(Name="BlazorTextEditorIconHeight")]
    public int HeightInPixels { get; set; } = 16;
}