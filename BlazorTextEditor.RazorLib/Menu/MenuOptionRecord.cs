namespace BlazorTextEditor.RazorLib.Menu;

public record MenuOptionRecord(
    string DisplayName,
    MenuOptionKind MenuOptionKind,
    Action? OnClick = null,
    MenuRecord? SubMenu = null,
    Type? WidgetRendererType = null,
    Dictionary<string, object?>? WidgetParameters = null);