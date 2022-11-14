using BlazorTextEditor.RazorLib.TreeView.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TreeView.Keymap;

public interface ITreeViewKeymap
{
    public bool TryMapKey(
        KeyboardEventArgs keyboardEventArgs, 
        out TreeViewCommand? treeViewCommand);
}