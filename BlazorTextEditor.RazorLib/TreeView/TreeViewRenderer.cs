namespace BlazorTextEditor.RazorLib.TreeView;

public record TreeViewRenderer(
    Type DynamicComponentType,
    Dictionary<string, object?> DynamicComponentParameters);