using BlazorTextEditor.RazorLib.Store.TreeViewCase;

namespace BlazorTextEditor.RazorLib.TreeView.Commands;

public interface ITreeViewCommandParameter
{
    public TreeViewState TreeViewState { get; }
}