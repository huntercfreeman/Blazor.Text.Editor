namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public static class ThemeFacts
{
    public static readonly Theme VisualStudioDarkClone = new(
        ThemeKey.NewThemeKey(),
        "bte_dark-theme-visual-studio",
        "Visual Studio Dark Clone",
        ContrastKind.Default,
        ColorKind.Dark);

    public static readonly Theme VisualStudioLightClone = new(
        ThemeKey.NewThemeKey(),
        "bte_light-theme-visual-studio",
        "Visual Studio Light Clone",
        ContrastKind.Default,
        ColorKind.Light);
    
    public static readonly Theme BlazorTextEditorLightTheme = new(
        ThemeKey.NewThemeKey(),
        "bte_light-theme",
        "Blazor Text Editor Light Theme",
        ContrastKind.Default,
        ColorKind.Light);
    
    public static readonly Theme Unset = new(
        ThemeKey.NewThemeKey(),
        string.Empty,
        "Unset",
        ContrastKind.Default,
        ColorKind.Dark);
}