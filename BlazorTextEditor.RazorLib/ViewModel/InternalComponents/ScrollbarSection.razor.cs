using BlazorTextEditor.RazorLib.Model;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

public partial class ScrollbarSection : ComponentBase
{
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
}