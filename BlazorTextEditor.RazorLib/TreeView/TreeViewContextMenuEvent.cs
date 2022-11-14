using BlazorTextEditor.RazorLib.JavaScriptObjects;

namespace BlazorTextEditor.RazorLib.TreeView;

public record TreeViewContextMenuEvent(
    TreeView TreeView,
    Action CloseContextMenu,
    ContextMenuFixedPosition ContextMenuFixedPosition);