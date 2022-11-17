using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TreeViewCase;
using BlazorTextEditor.RazorLib.TreeView.Commands;
using BlazorTextEditor.RazorLib.TreeView.Keymap;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.TreeView;

public partial class TreeViewStateDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TreeViewStateContainer, TreeViewState?> TreeViewStateContainerWrap { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewStateKey TreeViewStateKey { get; set; } = null!;
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    /// <summary>
    /// If a consumer of the TreeView component does not have logic
    /// for their own DropdownComponent then one can provide a
    /// RenderFragment and a dropdown will be rendered for the consumer
    /// then their RenderFragment is rendered within that dropdown.
    /// <br/><br/>
    /// If one has their own DropdownComponent it is recommended
    /// they use <see cref="OnContextMenuFunc"/> instead.
    /// </summary>
    [Parameter]
    public RenderFragment<TreeViewContextMenuEvent>? OnContextMenuRenderFragment { get; set; }
    /// <summary>
    /// If a consumer of the TreeView component does not have logic
    /// for their own DropdownComponent it is recommended to use
    /// <see cref="OnContextMenuRenderFragment"/>
    /// <br/><br/>
    /// <see cref="OnContextMenuFunc"/> allows one to be notified of
    /// the ContextMenu event along with the necessary parameters
    /// by being given <see cref="TreeViewContextMenuEvent"/>
    /// </summary>
    [Parameter]
    public Func<TreeViewContextMenuEvent, Task>? OnContextMenuFunc { get; set; }
    [Parameter]
    public ITreeViewKeymap? TreeViewKeymap { get; set; }
    [Parameter]
    public TreeViewMouseEventRegistrar? TreeViewMouseEventRegistrar { get; set; }

    private TreeViewContextMenuEvent? _treeViewContextMenuEvent;

    private string ContextMenuCssStyleString => GetContextMenuCssStyleString();

    protected override void OnInitialized()
    {
        TreeViewStateContainerWrap.Select(tvsc =>
        {
            _ = tvsc.TreeViewStatesMap
                .TryGetValue(TreeViewStateKey, out var treeViewState);

            return treeViewState;
        });

        base.OnInitialized();
    }

    private int GetRootDepth(TreeView rootNode)
    {
        return rootNode is TreeViewAdhoc
            ? -1
            : 0;
    }

    private async Task HandleTreeViewOnKeyDownWithPreventScroll(
        KeyboardEventArgs keyboardEventArgs,
        TreeViewState? treeViewState)
    {
        if (treeViewState is null)
            return;

        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                TreeViewService.MoveActiveSelectionLeft(treeViewState.TreeViewStateKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                TreeViewService.MoveActiveSelectionDown(treeViewState.TreeViewStateKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                TreeViewService.MoveActiveSelectionUp(treeViewState.TreeViewStateKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                TreeViewService.MoveActiveSelectionRight(treeViewState.TreeViewStateKey);
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                TreeViewService.MoveActiveSelectionHome(treeViewState.TreeViewStateKey);
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                TreeViewService.MoveActiveSelectionEnd(treeViewState.TreeViewStateKey);
                break;
        }
        
        if (TreeViewKeymap is not null && 
            TreeViewKeymap.TryMapKey(keyboardEventArgs, out var command) &&
            command is not null)
        {
            await command.DoAsyncFunc(
                new TreeViewCommandParameter(
                    treeViewState));
        }
    }

    private async Task HandleTreeViewOnContextMenu(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView? treeViewMouseWasOver)
    {
        if (treeViewState is null)
            return;

        ContextMenuFixedPosition contextMenuFixedPosition;
        TreeView contextMenuTargetTreeView;

        if (mouseEventArgs.Button == -1)
        {
            if (treeViewState.ActiveNode is null)
                return;

            // If dedicated context menu button
            // or shift + F10
            // was pressed as opposed to
            // a mouse RightClick
            //
            // Use JavaScript to determine the ContextMenu
            // position

            contextMenuFixedPosition = await JsRuntime
                .InvokeAsync<ContextMenuFixedPosition>(
                    "blazorTextEditor.getTreeViewContextMenuFixedPosition",
                    TreeViewService.GetTreeContainerElementId(treeViewState.TreeViewStateKey),
                    TreeViewService.GetNodeElementId(
                        treeViewState.ActiveNode));

            contextMenuTargetTreeView = treeViewState.ActiveNode;
        }
        else
        {
            // If a mouse RightClick caused
            // the event then use the MouseEventArgs
            // to determine the ContextMenu
            // position

            if (treeViewMouseWasOver is null)
            {
                // 'whitespace' of the TreeView was right clicked
                // as opposed to a TreeView node and the event
                // should be ignored.

                return;
            }

            contextMenuFixedPosition = new ContextMenuFixedPosition(
                true,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY);

            contextMenuTargetTreeView = treeViewMouseWasOver;
        }

        _treeViewContextMenuEvent = new TreeViewContextMenuEvent(
            contextMenuTargetTreeView,
            () =>
            {
                _treeViewContextMenuEvent = null;
                InvokeAsync(StateHasChanged);
            },
            contextMenuFixedPosition);

        if (OnContextMenuFunc is not null)
            await OnContextMenuFunc.Invoke(_treeViewContextMenuEvent);

        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleTreeViewOnClick(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView? treeViewMouseWasOver)
    {
        if (treeViewState is null)
            return;

        var onClick = 
            TreeViewMouseEventRegistrar?.OnClick;
        
        if (onClick is not null)
        {
            await onClick.Invoke(new TreeViewMouseEventParameter(
                new TreeViewCommandParameter(treeViewState),
                treeViewMouseWasOver,
                mouseEventArgs));
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleTreeViewOnDoubleClick(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView? treeViewMouseWasOver)
    {
        if (treeViewState is null)
            return;

        var onDoubleClick = 
            TreeViewMouseEventRegistrar?.OnDoubleClick;
        
        if (onDoubleClick is not null)
        {
            await onDoubleClick.Invoke(new TreeViewMouseEventParameter(
                new TreeViewCommandParameter(treeViewState),
                treeViewMouseWasOver,
                mouseEventArgs));
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleTreeViewOnMouseDown(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView? treeViewMouseWasOver)
    {
        if (treeViewState is null)
            return;

        var onMouseDown = 
            TreeViewMouseEventRegistrar?.OnMouseDown;
        
        if (onMouseDown is not null)
        {
            await onMouseDown.Invoke(new TreeViewMouseEventParameter(
                new TreeViewCommandParameter(treeViewState),
                treeViewMouseWasOver,
                mouseEventArgs));
        }
        
        await InvokeAsync(StateHasChanged);
    }

    private string GetHasActiveNodeCssClass(TreeViewState? treeViewState)
    {
        if (treeViewState is null ||
            treeViewState.ActiveNode is null)
        {
            return string.Empty;
        }

        return "btv_active";
    }

    private string GetContextMenuCssStyleString()
    {
        if (_treeViewContextMenuEvent is null)
        {
            // This css should never get applied as the ContextMenu
            // user interface is wrapped in an if statement that
            // checks for _contextMenuFixedPosition not being null
            //
            // Logic is here to supress the rightfully so
            // nullable reference warning.
            return "display: none;";
        }

        var left =
            $"left: {_treeViewContextMenuEvent.ContextMenuFixedPosition.LeftPositionInPixels}px;";

        var top =
            $"top: {_treeViewContextMenuEvent.ContextMenuFixedPosition.TopPositionInPixels}px;";

        return $"{left} {top}";
    }
}