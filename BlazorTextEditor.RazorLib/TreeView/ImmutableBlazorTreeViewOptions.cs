namespace BlazorTextEditor.RazorLib.TreeView;

public class ImmutableBlazorTreeViewOptions : IBlazorTreeViewOptions
{
    public ImmutableBlazorTreeViewOptions(BlazorTreeViewOptions blazorTreeViewOptions)
    {
        InitializeFluxor = blazorTreeViewOptions.InitializeFluxor;
    }

    public bool InitializeFluxor { get; }
}