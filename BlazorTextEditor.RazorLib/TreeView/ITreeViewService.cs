using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TreeViewCase;

namespace BlazorTextEditor.RazorLib.TreeView;

public interface ITreeViewService : IDisposable
{
    public TreeViewStateContainer TreeViewStateContainer { get; }
    public ImmutableArray<TreeViewState> TreeViewStates { get; }

    public event Action? OnTreeViewStateContainerChanged;

    /// <summary>
    /// If a <see cref="TreeViewState"/> with the
    /// <see cref="TreeViewStateKey"/> that exists on the
    /// provided parameter <see cref="treeViewState"/>
    /// nothing will occur (no duplicate key exceptions will occur).
    /// </summary>
    public void RegisterTreeViewState( 
        TreeViewState treeViewState);
    
    public void SetRoot(
        TreeViewStateKey treeViewStateKey,
        TreeView treeView);
    
    public bool TryGetTreeViewState(
        TreeViewStateKey treeViewStateKey,
        out TreeViewState? treeViewState);
    
    public void ReRenderNode(
        TreeViewStateKey treeViewStateKey,
        TreeView node);

    public void AddChildNode(
        TreeViewStateKey treeViewStateKey,
        TreeView parent,
        TreeView child);
    
    public void SetActiveNode(
        TreeViewStateKey treeViewStateKey,
        TreeView? nextActiveNode);
    
    public void MoveActiveSelectionLeft(
        TreeViewStateKey treeViewStateKey);
    
    public void MoveActiveSelectionDown(
        TreeViewStateKey treeViewStateKey);
    
    public void MoveActiveSelectionUp(
        TreeViewStateKey treeViewStateKey);
    
    public void MoveActiveSelectionRight(
        TreeViewStateKey treeViewStateKey);
    
    public void MoveActiveSelectionHome(
        TreeViewStateKey treeViewStateKey);
    
    public void MoveActiveSelectionEnd(
        TreeViewStateKey treeViewStateKey);
    
    public string GetNodeElementId(
        TreeView treeView);

    public string GetTreeContainerElementId(
        TreeViewStateKey treeViewStateKey);
    
    public void DisposeTreeViewState(
        TreeViewStateKey treeViewStateKey);
}