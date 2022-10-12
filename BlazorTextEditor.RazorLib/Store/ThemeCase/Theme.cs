namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public record Theme(
    ThemeKey ThemeKey,
    string CssClassString,
    string DisplayName,
    ContrastKind ContrastKind,
    ColorKind ColorKind);