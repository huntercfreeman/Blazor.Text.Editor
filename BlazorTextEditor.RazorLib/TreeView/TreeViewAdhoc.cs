namespace BlazorTextEditor.RazorLib.TreeView;

/// <summary>
/// <see cref="TreeViewAdhoc"/> is used when the
/// consumer of the component does not want to show the root.
/// <br/><br/>
/// The TreeViews were designed with a root consisting of 1 node.
/// To get around this <see cref="TreeViewAdhoc"/> can be used
/// to have that top level root node be invisible to the user.
/// </summary>
public class TreeViewAdhoc : TreeViewBase<byte>
{
    public TreeViewAdhoc(byte value)
        : base(value)
    {
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewAdhoc treeViewAdhoc)
        {
            return false;
        }

        return treeViewAdhoc.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item;
    }
    
    public static TreeViewAdhoc ConstructTreeViewAdhoc()
    {
        return new TreeViewAdhoc(byte.MinValue)
        {
            Parent = null,
            IsExpandable = false,
            IsExpanded= true,
            IndexAmongSiblings = 0,
            IsRoot = true,
            IsHidden = true,
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
        };
    }
    
    public static TreeViewAdhoc ConstructTreeViewAdhoc(params TreeView[] treeViews)
    {
        var treeViewAdhoc = ConstructTreeViewAdhoc();

        var children = treeViews.ToList();

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];

            child.IndexAmongSiblings = i;
            child.Parent = treeViewAdhoc;
        }

        treeViewAdhoc.Children = children;

        return treeViewAdhoc;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewAdhocDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewAdhocDisplay.TreeViewAdhoc),
                    this
                },
            });
    }

    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeView> treeViews)
    {
        // This method is meant to do nothing in this case.
    }
}