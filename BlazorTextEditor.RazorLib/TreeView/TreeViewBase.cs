namespace BlazorTextEditor.RazorLib.TreeView;

/// <summary>
/// Implement the abstract class <see cref="TreeViewBase{T}"/>
/// in order to make a TreeView.
/// <br/><br/>
/// An abstract class is used because a good deal of customization
/// is required on a per TreeView basis depending on what data type
/// one displays in that TreeView.
/// </summary>
public abstract class TreeViewBase<T> : TreeView
{
    public TreeViewBase(T? item)
    {
        Item = item;
    }
    
    public T? Item { get; }
    public override object? UntypedItem => Item;
    public override Type ItemType => typeof(T);
}