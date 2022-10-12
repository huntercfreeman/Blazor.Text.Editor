using BlazorTextEditor.RazorLib.Store.ThemeCase;

namespace BlazorTextEditor.RazorLib;

public interface IThemeService : IDisposable
{
    public ThemeStates ThemeStates { get; }
    
    public event EventHandler? OnThemeStatesChanged;

    public void RegisterTheme(Theme theme);
    public void DisposeTheme(ThemeKey themeKey);
}