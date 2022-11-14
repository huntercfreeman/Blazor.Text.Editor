namespace BlazorTextEditor.RazorLib.TreeView;

public record TreeViewChangedKey(Guid Guid)
{
    public static readonly TreeViewChangedKey Empty = new TreeViewChangedKey(Guid.Empty);
    
    public static TreeViewChangedKey NewTreeViewChangedKey()
    {
        return new TreeViewChangedKey(Guid.NewGuid());
    }
}