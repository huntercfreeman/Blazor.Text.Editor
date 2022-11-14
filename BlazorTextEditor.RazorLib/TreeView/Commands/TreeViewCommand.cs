namespace BlazorTextEditor.RazorLib.TreeView.Commands;

public class TreeViewCommand
{
    public TreeViewCommand(
        Func<ITreeViewCommandParameter, Task> doAsyncFunc)
    {
        DoAsyncFunc = doAsyncFunc;
    }

    public Func<ITreeViewCommandParameter, Task> DoAsyncFunc { get; }
}