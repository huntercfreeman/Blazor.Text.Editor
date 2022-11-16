using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TreeView;

public partial class TreeViewAdhocDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeView TreeViewAdhoc { get; set; } = null!;
}