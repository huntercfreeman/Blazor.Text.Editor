using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TreeViewCase;

[FeatureState]
public partial record TreeViewStateContainer(
    ImmutableDictionary<TreeViewStateKey, TreeViewState> TreeViewStatesMap)
{
    public TreeViewStateContainer() : this(
        ImmutableDictionary<TreeViewStateKey, TreeViewState>.Empty)
    {
        
    }

    public record RegisterTreeViewStateAction( 
        TreeViewState TreeViewState);
    
    public record ReplaceTreeViewStateAction( 
        TreeViewStateKey TreeViewStateKey,
        TreeViewState TreeViewState);
    
    public record WithRootAction(
        TreeViewStateKey TreeViewStateKey,
        TreeView.TreeView TreeView);
    
    public record TryGetRootAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record AddChildNodeAction(
        TreeViewStateKey TreeViewStateKey,
        TreeView.TreeView Parent,
        TreeView.TreeView Child);
    
    public record ReRenderNodeAction(
        TreeViewStateKey TreeViewStateKey,
        TreeView.TreeView Node);
    
    public record SetActiveNodeAction(
        TreeViewStateKey TreeViewStateKey,
        TreeView.TreeView NextActiveNode);
    
    public record MoveActiveSelectionLeftAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record MoveActiveSelectionDownAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record MoveActiveSelectionUpAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record MoveActiveSelectionRightAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record MoveActiveSelectionHomeAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record MoveActiveSelectionEndAction(
        TreeViewStateKey TreeViewStateKey);
    
    public record DisposeTreeViewStateAction(
        TreeViewStateKey TreeViewStateKey);
}