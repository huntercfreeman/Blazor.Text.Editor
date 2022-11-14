using BlazorTextEditor.RazorLib.Store.TreeViewCase;

namespace BlazorTextEditor.RazorLib.TreeView.Commands;

public class TreeViewCommandParameter : ITreeViewCommandParameter
{
    public TreeViewCommandParameter(TreeViewState treeViewState)
    {
        TreeViewState = treeViewState;
    }

    public TreeViewState TreeViewState { get; }
}