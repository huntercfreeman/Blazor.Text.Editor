using BlazorTextEditor.RazorLib.TreeView.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TreeView;

/// <summary>
/// <see cref="MouseTargetedTreeView"/> refers to the
/// TreeView of which was underneath the cursor upon the event firing.
/// <br/><br/>
/// It is not to be confused with the actual active TreeView node.
/// As well <see cref="MouseTargetedTreeView"/> will be null if the mouse
/// event happened while the user's mouse was hovering over 'whitespace'
/// in the TreeView where no node was located.
/// </summary>
public record TreeViewMouseEventParameter(
    ITreeViewCommandParameter TreeViewCommandParameter,
    TreeView? MouseTargetedTreeView,
    MouseEventArgs MouseEventArgs);