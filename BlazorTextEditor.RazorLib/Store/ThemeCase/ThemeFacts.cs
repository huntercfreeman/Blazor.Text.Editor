namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public static class ThemeFacts
{
    public static readonly Theme VisualStudioLightClone = new(
        ThemeKey.NewThemeKey(),
        "bte_light-theme-visual-studio",
        "Visual Studio Light Clone",
        ContrastKind.Default,
        ColorKind.Light);
    
    public static readonly Theme BlazorTextEditorDarkTheme = new(
        ThemeKey.NewThemeKey(),
        "bte_dark-theme",
        "Blazor Text Editor Dark Theme",
        ContrastKind.Default,
        ColorKind.Dark);
    
    public static readonly Theme BlazorTextEditorLightTheme = new(
        ThemeKey.NewThemeKey(),
        "bte_light-theme",
        "Blazor Text Editor Light Theme",
        ContrastKind.Default,
        ColorKind.Light);
    
    /// <summary>
    /// <see cref="Unset"/> is a Visual Studio Dark Clone
    /// </summary>
    public static readonly Theme Unset = new(
        ThemeKey.NewThemeKey(),
        string.Empty,
        "Unset",
        ContrastKind.Default,
        ColorKind.Dark);
}