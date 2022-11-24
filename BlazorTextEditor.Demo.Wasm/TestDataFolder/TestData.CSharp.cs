namespace BlazorTextEditor.Demo.Wasm.TestDataFolder;

public static partial class TestData
{
    public static class CSharp
    {
        public const string EXAMPLE_TEXT_8_LINES = @"namespace BlazorTreeView.RazorLib;
public class MyClass
{
    public void MyMethod()
    {
        return;
    }
}";
        
        public const string EXAMPLE_TEXT_173_LINES = @"using System.Collections.Immutable;
using BlazorTreeView.RazorLib.Store.TreeViewCase;
using Fluxor;

namespace BlazorTreeView.RazorLib;

public class TreeViewService : ITreeViewService
{
    private readonly IDispatcher _dispatcher;
    private readonly IState<TreeViewStateContainer> _treeViewStateContainerWrap;

    public TreeViewService(
        IState<TreeViewStateContainer> treeViewStateContainer,
        IDispatcher dispatcher)
    {
        _treeViewStateContainerWrap = treeViewStateContainer;
        _dispatcher = dispatcher;

        _treeViewStateContainerWrap.StateChanged += TreeViewStateContainerOnStateChanged;
    }

    public TreeViewStateContainer TreeViewStateContainer => 
        _treeViewStateContainerWrap.Value;
    
    public ImmutableArray<TreeViewState> TreeViewStates =>
        _treeViewStateContainerWrap.Value.TreeViewStatesMap.Values.ToImmutableArray();
    
    public event Action? OnTreeViewStateContainerChanged;
    
    public void RegisterTreeViewState(TreeViewState treeViewState)
    {
        var registerTreeViewStateAction = new TreeViewStateContainer.RegisterTreeViewStateAction(
            treeViewState);
        
        _dispatcher.Dispatch(registerTreeViewStateAction);
    }
    
    public void ReplaceTreeViewState(
        TreeViewStateKey treeViewStateKey, 
        TreeViewState treeViewState)
    {
        var replaceTreeViewStateAction = new TreeViewStateContainer.ReplaceTreeViewStateAction(
            treeViewStateKey,
            treeViewState);
        
        _dispatcher.Dispatch(replaceTreeViewStateAction);
    }

    public void SetRoot(TreeViewStateKey treeViewStateKey, TreeView treeView)
    {
        var withRootAction = new TreeViewStateContainer.WithRootAction(
            treeViewStateKey, 
            treeView);
        
        _dispatcher.Dispatch(withRootAction);
    }

    public bool TryGetTreeViewState(
        TreeViewStateKey treeViewStateKey,
        out TreeViewState? treeViewState)
    {
        return _treeViewStateContainerWrap.Value.TreeViewStatesMap
            .TryGetValue(treeViewStateKey, out treeViewState);
    }

    public void ReRenderNode(
        TreeViewStateKey treeViewStateKey,
        TreeView node)
    {
        var replaceNodeAction = new TreeViewStateContainer.ReRenderNodeAction(
            treeViewStateKey,
            node);
        
        _dispatcher.Dispatch(replaceNodeAction);
    }

    public void AddChildNode(
        TreeViewStateKey treeViewStateKey,
        TreeView parent,
        TreeView child)
    {
        var addChildNodeAction = new TreeViewStateContainer.AddChildNodeAction(
            treeViewStateKey,
            parent,
            child);
        
        _dispatcher.Dispatch(addChildNodeAction);
    }

    public void SetActiveNode(
        TreeViewStateKey treeViewStateKey,
        TreeView nextActiveNode)
    {
        var setActiveNodeAction = new TreeViewStateContainer.SetActiveNodeAction(
            treeViewStateKey,
            nextActiveNode);
        
        _dispatcher.Dispatch(setActiveNodeAction);
    }

    public void MoveActiveSelectionLeft(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionLeftAction = new TreeViewStateContainer.MoveActiveSelectionLeftAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionLeftAction);
    }

    public void MoveActiveSelectionDown(
        TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionDownAction = new TreeViewStateContainer.MoveActiveSelectionDownAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionDownAction);
    }

    public void MoveActiveSelectionUp(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionUpAction = new TreeViewStateContainer.MoveActiveSelectionUpAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionUpAction);
    }

    public void MoveActiveSelectionRight(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionRightAction = new TreeViewStateContainer.MoveActiveSelectionRightAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionRightAction);
    }

    public void MoveActiveSelectionHome(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionHomeAction = new TreeViewStateContainer.MoveActiveSelectionHomeAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionHomeAction);
    }

    public void MoveActiveSelectionEnd(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionEndAction = new TreeViewStateContainer.MoveActiveSelectionEndAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionEndAction);
    }

    public string GetNodeElementId(
        TreeView treeView)
    {
        return $""btv_node-{treeView.Id}"";
    }

    public void DisposeTreeViewState(TreeViewStateKey treeViewStateKey)
    {
        var disposeTreeViewStateAction = new TreeViewStateContainer.DisposeTreeViewStateAction(
            treeViewStateKey);
        
        _dispatcher.Dispatch(disposeTreeViewStateAction);
    }
    
    private void TreeViewStateContainerOnStateChanged(object? sender, EventArgs e)
    {
        OnTreeViewStateContainerChanged?.Invoke();
    }
    
    public void Dispose()
    {
        _treeViewStateContainerWrap.StateChanged -= TreeViewStateContainerOnStateChanged;
    }
}";
    }
}