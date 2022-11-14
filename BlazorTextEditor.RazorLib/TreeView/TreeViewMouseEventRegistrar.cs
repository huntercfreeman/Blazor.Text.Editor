namespace BlazorTextEditor.RazorLib.TreeView;

public class TreeViewMouseEventRegistrar
{
    public Func<TreeViewMouseEventParameter, Task>? OnClick { get; init; }
    public Func<TreeViewMouseEventParameter, Task>? OnDoubleClick { get; init; }
    public Func<TreeViewMouseEventParameter, Task>? OnMouseDown { get; init; }
}