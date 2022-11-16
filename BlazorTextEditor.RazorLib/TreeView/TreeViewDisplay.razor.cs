using BlazorTextEditor.RazorLib.Store.TreeViewCase;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TreeView;

public partial class TreeViewDisplay : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;
    [CascadingParameter(Name="HandleTreeViewOnContextMenu")]
    public Func<
        MouseEventArgs, TreeViewState?, TreeView?, Task> 
        HandleTreeViewOnContextMenu { get; set; } = null!;
    [CascadingParameter(Name="HandleTreeViewOnClick")]
    public Func<
        MouseEventArgs, TreeViewState?, TreeView?, Task> 
        HandleTreeViewOnClick { get; set; } = null!;
    [CascadingParameter(Name="HandleTreeViewOnDoubleClick")]
    public Func<
        MouseEventArgs, TreeViewState?, TreeView?, Task> 
        HandleTreeViewOnDoubleClick { get; set; } = null!;
    [CascadingParameter(Name="HandleTreeViewOnMouseDown")]
    public Func<
        MouseEventArgs, TreeViewState?, TreeView?, Task> 
        HandleTreeViewOnMouseDown { get; set; } = null!;
    [CascadingParameter(Name="OffsetPerDepthInPixels")]
    public int OffsetPerDepthInPixels { get; set; } = 12;
    [CascadingParameter(Name="BlazorTreeViewIconWidth")]
    public int WidthOfTitleExpansionChevron { get; set; } = 16;

    [Parameter, EditorRequired]
    public TreeView TreeView { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Depth { get; set; }

    private ElementReference? _treeViewTitleElementReference;
    private TreeViewChangedKey _previousTreeViewChangedKey = TreeViewChangedKey.Empty;

    private bool _previousIsActive;
    
    private int OffsetInPixels => OffsetPerDepthInPixels * Depth;

    private bool IsActive => TreeViewState.ActiveNode is not null &&
                             TreeViewState.ActiveNode.Id == TreeView.Id;
    private string IsActiveCssClass => IsActive
        ? "btv_active"
        : string.Empty;

    protected override bool ShouldRender()
    {
        if (_previousTreeViewChangedKey != TreeView.TreeViewChangedKey)
        {
            _previousTreeViewChangedKey = TreeView.TreeViewChangedKey;
            return true;
        }
        
        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var localIsActive = IsActive;
        
        var localTreeViewTitleElementReference = 
            _treeViewTitleElementReference;
        
        if (_previousIsActive != localIsActive)
        {
            _previousIsActive = localIsActive;

            if (localIsActive &&
                localTreeViewTitleElementReference.HasValue)
            {
                await localTreeViewTitleElementReference.Value.FocusAsync();
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleTitleOnMouseDown(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView treeView)
    {
        TreeViewService.SetActiveNode(
            TreeViewState.TreeViewStateKey,
            TreeView);
        
        TreeViewService.ReRenderNode(
            TreeViewState.TreeViewStateKey,
            TreeView);

        await ManuallyPropagateOnMouseDown(
            mouseEventArgs,
            treeViewState,
            treeView);
    }

    private async Task HandleExpansionChevronOnMouseDown(
        TreeView localTreeView)
    {
        if (!localTreeView.IsExpandable)
            return;
        
        localTreeView.IsExpanded = !localTreeView.IsExpanded;

        if (localTreeView.IsExpanded)
            await localTreeView.LoadChildrenAsync();
        
        TreeViewService.ReRenderNode(
            TreeViewState.TreeViewStateKey,
            localTreeView);
    }

    /// <summary>
    /// Capture the variables on event as to
    /// avoid referenced object mutating during the event.
    /// </summary>
    private async Task ManuallyPropagateOnContextMenu(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView treeView)
    {
        await HandleTreeViewOnContextMenu.Invoke(
            mouseEventArgs,
            treeViewState,
            treeView);
    }
    
    private async Task ManuallyPropagateOnClick(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView treeView)
    {
        await HandleTreeViewOnClick.Invoke(
            mouseEventArgs,
            treeViewState,
            treeView);
    }
    
    private async Task ManuallyPropagateOnDoubleClick(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView treeView)
    {
        await HandleTreeViewOnDoubleClick.Invoke(
            mouseEventArgs,
            treeViewState,
            treeView);
    }
    
    private async Task ManuallyPropagateOnMouseDown(
        MouseEventArgs mouseEventArgs,
        TreeViewState? treeViewState,
        TreeView treeView)
    {
        await HandleTreeViewOnMouseDown.Invoke(
            mouseEventArgs,
            treeViewState,
            treeView);
    }

    private async Task HandleOnKeyDown(
        KeyboardEventArgs keyboardEventArgs, 
        TreeViewState treeViewState, 
        TreeView treeView)
    {
        if (keyboardEventArgs.Key == "ContextMenu")
        {
            var mouseEventArgs = new MouseEventArgs
            {
                Button = -1
            };

            await ManuallyPropagateOnContextMenu(
                mouseEventArgs, 
                TreeViewState, 
                TreeView);
        }
    }
}