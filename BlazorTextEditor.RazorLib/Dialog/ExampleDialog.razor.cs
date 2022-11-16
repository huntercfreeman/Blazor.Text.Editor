using BlazorTextEditor.RazorLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Dialog;

public partial class ExampleDialog : ComponentBase
{
    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public string Message { get; set; } = null!;
}