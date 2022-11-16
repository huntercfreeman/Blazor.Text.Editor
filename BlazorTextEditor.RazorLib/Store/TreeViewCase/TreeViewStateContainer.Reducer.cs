using BlazorTextEditor.RazorLib.TreeView;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TreeViewCase;

public partial record TreeViewStateContainer
{
    private class TreeViewStateContainerReducer
    {
        [ReducerMethod]
        public static TreeViewStateContainer ReduceRegisterTreeViewStateAction(
            TreeViewStateContainer inTreeViewStateContainer,
            RegisterTreeViewStateAction registerTreeViewStateAction)
        {
            if (inTreeViewStateContainer.TreeViewStatesMap
                .ContainsKey(registerTreeViewStateAction.TreeViewState.TreeViewStateKey))
            {
                return inTreeViewStateContainer;
            }

            var nextMap = inTreeViewStateContainer
                .TreeViewStatesMap.Add(
                    registerTreeViewStateAction.TreeViewState.TreeViewStateKey,
                    registerTreeViewStateAction.TreeViewState);

            return inTreeViewStateContainer with
            {
                TreeViewStatesMap = nextMap
            };
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceDisposeTreeViewStateAction(
            TreeViewStateContainer inTreeViewStateContainer,
            DisposeTreeViewStateAction disposeTreeViewStateAction)
        {
            var nextMap = inTreeViewStateContainer
                .TreeViewStatesMap.Remove(
                    disposeTreeViewStateAction.TreeViewStateKey);

            return inTreeViewStateContainer with
            {
                TreeViewStatesMap = nextMap
            };
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceWithRootAction(
            TreeViewStateContainer inTreeViewStateContainer,
            WithRootAction withRootAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    withRootAction.TreeViewStateKey, out var treeViewState))
            {
                return inTreeViewStateContainer;
            }

            var nextTreeViewState = treeViewState with
            {
                RootNode = withRootAction.TreeView,
                ActiveNode = withRootAction.TreeView
            };

            var nextMap = 
                inTreeViewStateContainer.TreeViewStatesMap
                    .SetItem(
                        withRootAction.TreeViewStateKey, 
                        nextTreeViewState);

            return inTreeViewStateContainer with
            {
                TreeViewStatesMap = nextMap
            };
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceAddChildNodeAction(
            TreeViewStateContainer inTreeViewStateContainer,
            AddChildNodeAction addChildNodeAction)
        {
            var parent = addChildNodeAction.Parent;
            var child = addChildNodeAction.Child;

            child.Parent = parent;
            child.IndexAmongSiblings = parent.Children.Count;
            child.TreeViewChangedKey = TreeViewChangedKey
                .NewTreeViewChangedKey();
            
            parent.Children.Add(child);

            var rerenderNodeAction = new ReRenderNodeAction(
                addChildNodeAction.TreeViewStateKey, 
                parent);
            
            return ReduceReRenderNodeAction(
                inTreeViewStateContainer,
                rerenderNodeAction);
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceReRenderNodeAction(
            TreeViewStateContainer inTreeViewStateContainer,
            ReRenderNodeAction reRenderNodeAction)
        {
            MarkForRerender(reRenderNodeAction.Node);

            return inTreeViewStateContainer with { };
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceSetActiveNodeAction(
            TreeViewStateContainer inTreeViewStateContainer,
            SetActiveNodeAction setActiveNodeAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    setActiveNodeAction.TreeViewStateKey, out var treeViewState))
            {
                return inTreeViewStateContainer;
            }
            
            if (treeViewState.ActiveNode is not null)
                MarkForRerender(treeViewState.ActiveNode);

            if (setActiveNodeAction.NextActiveNode is not null)
                MarkForRerender(setActiveNodeAction.NextActiveNode);

            var nextTreeViewState = treeViewState with
            {
                ActiveNode = setActiveNodeAction.NextActiveNode
            };
            
            var nextMap = 
                inTreeViewStateContainer.TreeViewStatesMap
                    .SetItem(
                        setActiveNodeAction.TreeViewStateKey, 
                        nextTreeViewState);

            return inTreeViewStateContainer with
            {
                TreeViewStatesMap = nextMap
            };           
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionLeftAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionLeftAction moveActiveSelectionLeftAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionLeftAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            if (treeViewState.ActiveNode.IsExpanded && 
                treeViewState.ActiveNode.IsExpandable)
            {
                treeViewState.ActiveNode.IsExpanded = false;

                var reRenderNodeAction = new ReRenderNodeAction(
                    treeViewState.TreeViewStateKey,
                    treeViewState.ActiveNode);
                
                inTreeViewStateContainer = ReduceReRenderNodeAction(
                    inTreeViewStateContainer,
                    reRenderNodeAction);
            }
            else if (treeViewState.ActiveNode.Parent is not null)
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    treeViewState.TreeViewStateKey,
                    treeViewState.ActiveNode.Parent);
                
                inTreeViewStateContainer = ReduceSetActiveNodeAction(
                    inTreeViewStateContainer,
                    setActiveNodeAction);
            }

            return inTreeViewStateContainer;
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionDownAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionDownAction moveActiveSelectionDownAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionDownAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            if (treeViewState.ActiveNode.IsExpanded &&
                treeViewState.ActiveNode.Children.Any())
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    treeViewState.TreeViewStateKey,
                    treeViewState.ActiveNode.Children[0]);
                
                return ReduceSetActiveNodeAction(
                    inTreeViewStateContainer,
                    setActiveNodeAction);
            }
            else
            {
                var target = treeViewState.ActiveNode;
            
                while (target.Parent is not null &&
                       target.IndexAmongSiblings == 
                       target.Parent.Children.Count - 1)
                {
                    target = target.Parent;
                }

                if (target.Parent is null ||
                    target.IndexAmongSiblings == 
                    target.Parent.Children.Count - 1)
                {
                    return inTreeViewStateContainer;
                }
            
                var setActiveNodeAction = new SetActiveNodeAction(
                    treeViewState.TreeViewStateKey,
                    target.Parent.Children[
                        target.IndexAmongSiblings + 1]);

                return ReduceSetActiveNodeAction(
                    inTreeViewStateContainer,
                    setActiveNodeAction);
            }
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionUpAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionUpAction moveActiveSelectionUpAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionUpAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            if (treeViewState.ActiveNode.Parent is null)
                return inTreeViewStateContainer;

            if (treeViewState.ActiveNode.IndexAmongSiblings == 0)
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    treeViewState.TreeViewStateKey,
                    treeViewState.ActiveNode.Parent);

                return ReduceSetActiveNodeAction(
                    inTreeViewStateContainer,
                    setActiveNodeAction);
            }
            else
            {
                var target = treeViewState
                    .ActiveNode.Parent.Children[
                        treeViewState.ActiveNode.IndexAmongSiblings - 1];
            
                while (target.IsExpanded && 
                       target.Children.Any())
                {
                    target = target.Children.Last();
                }

                var setActiveNodeAction = new SetActiveNodeAction(
                    treeViewState.TreeViewStateKey,
                    target);

                return ReduceSetActiveNodeAction(
                    inTreeViewStateContainer,
                    setActiveNodeAction);
            }
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionRightAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionRightAction moveActiveSelectionRightAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionRightAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            if (treeViewState.ActiveNode.IsExpanded)
            {
                if (treeViewState.ActiveNode.Children.Any())
                {
                    var setActiveNodeAction = new SetActiveNodeAction(
                        treeViewState.TreeViewStateKey,
                        treeViewState.ActiveNode.Children[0]);

                    return ReduceSetActiveNodeAction(
                        inTreeViewStateContainer,
                        setActiveNodeAction);
                }
            }
            else if (treeViewState.ActiveNode.IsExpandable)
            {
                treeViewState.ActiveNode.IsExpanded = true;

                var reRenderNodeAction = new ReRenderNodeAction(
                    treeViewState.TreeViewStateKey,
                    treeViewState.ActiveNode);

                treeViewState.ActiveNode
                    .LoadChildrenAsync()
                    .Wait();

                return ReduceReRenderNodeAction(
                    inTreeViewStateContainer,
                    reRenderNodeAction);
            }
            
            return inTreeViewStateContainer;
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionHomeAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionHomeAction moveActiveSelectionHomeAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionHomeAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            TreeView.TreeView target;
            
            if (treeViewState.RootNode is TreeViewAdhoc)
            {
                if (treeViewState.RootNode.Children.Any())
                    target = treeViewState.RootNode.Children[0];
                else
                    target = treeViewState.RootNode;
            }
            else
            {
                target = treeViewState.RootNode;
            }
            
            var setActiveNodeAction = new SetActiveNodeAction(
                treeViewState.TreeViewStateKey,
                target);

            return ReduceSetActiveNodeAction(
                inTreeViewStateContainer,
                setActiveNodeAction);
        }
        
        [ReducerMethod]
        public static TreeViewStateContainer ReduceMoveActiveSelectionEndAction(
            TreeViewStateContainer inTreeViewStateContainer,
            MoveActiveSelectionEndAction moveActiveSelectionEndAction)
        {
            if (!inTreeViewStateContainer.TreeViewStatesMap.TryGetValue(
                    moveActiveSelectionEndAction.TreeViewStateKey, 
                    out var treeViewState) ||
                treeViewState.ActiveNode is null)
            {
                return inTreeViewStateContainer;
            }

            var target = treeViewState.RootNode;
            
            while (target.IsExpanded &&
                   target.Children.Any())
            {
                target = target.Children.Last();
            }

            var setActiveNodeAction = new SetActiveNodeAction(
                treeViewState.TreeViewStateKey,
                target);

            return ReduceSetActiveNodeAction(
                inTreeViewStateContainer,
                setActiveNodeAction);
        }
        
        public static void MarkForRerender(
            TreeView.TreeView treeView)
        {
            var markForRerenderTarget = treeView;
            
            while (markForRerenderTarget is not null)
            {
                markForRerenderTarget.TreeViewChangedKey = TreeViewChangedKey
                    .NewTreeViewChangedKey();

                markForRerenderTarget = markForRerenderTarget.Parent;
            }
        }
    }
}