using BlazorALaCarte.Shared.JavaScriptObjects;

namespace BlazorTextEditor.RazorLib.TreeView;

public record TreeViewContextMenuEvent(
    TreeView TreeView,
    Action CloseContextMenu,
    ContextMenuFixedPosition ContextMenuFixedPosition);