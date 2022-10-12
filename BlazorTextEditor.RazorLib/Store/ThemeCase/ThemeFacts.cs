namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public static class ThemeFacts
{
    public static readonly Theme BlazorTextEditorDark = new Theme(
        ThemeKey.NewThemeKey(),
        "bte_dark-theme",
        "BlazorTextEditor Dark Theme",
        ContrastKind.Default,
        ColorKind.Light);
    
    public static readonly Theme BlazorTextEditorLight = new Theme(
        ThemeKey.NewThemeKey(), 
        "bte_light-theme",
        "BlazorTextEditor Light Theme",
        ContrastKind.Default,
        ColorKind.Light);
    
    public static readonly Theme VisualStudioDarkClone = new Theme(
        ThemeKey.NewThemeKey(), 
        "bte_dark-theme-visual-studio",
        "Visual Studio Dark Clone",
        ContrastKind.Default,
        ColorKind.Dark);
    
    public static readonly Theme VisualStudioLightClone = new Theme(
        ThemeKey.NewThemeKey(), 
        "bte_light-theme-visual-studio",
        "Visual Studio Light Clone",
        ContrastKind.Default,
        ColorKind.Light);
}