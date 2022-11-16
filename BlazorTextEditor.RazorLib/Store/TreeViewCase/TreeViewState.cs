using BlazorTextEditor.RazorLib.TreeView;

namespace BlazorTextEditor.RazorLib.Store.TreeViewCase;

public record TreeViewState
{
    /// <summary>
    /// If <see cref="rootNode"/> is null then
    /// <see cref="TreeViewAdhoc.ConstructTreeViewAdhoc()"/>
    /// will be invoked and the return value
    /// will be used as the <see cref="RootNode"/>
    /// </summary>
    public TreeViewState(
        TreeViewStateKey treeViewStateKey, 
        TreeView.TreeView? rootNode, 
        TreeView.TreeView? activeNode)
    {
        rootNode ??= TreeViewAdhoc.ConstructTreeViewAdhoc();
        
        TreeViewStateKey = treeViewStateKey;
        RootNode = rootNode;
        ActiveNode = activeNode;
    }

    public TreeViewStateKey TreeViewStateKey { get; init; }
    public TreeView.TreeView RootNode { get; init; }
    public TreeView.TreeView? ActiveNode { get; init; }
}