using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public class ThemeService : IThemeService
{
    private readonly IDispatcher _dispatcher;
    private readonly IState<ThemeStates> _themeStates;

    public ThemeService(
        IState<ThemeStates> themeStates,
        IDispatcher dispatcher)
    {
        _themeStates = themeStates;
        _dispatcher = dispatcher;

        _themeStates.StateChanged += ThemeStatesOnStateChanged;
    }

    public ThemeStates ThemeStates => _themeStates.Value;

    public event Action? OnThemeStatesChanged;

    public void RegisterTheme(Theme theme)
    {
        _dispatcher.Dispatch(new RegisterThemeAction(theme));
    }

    public void DisposeTheme(ThemeKey themeKey)
    {
        _dispatcher.Dispatch(new DisposeThemeAction(themeKey));
    }

    private void ThemeStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnThemeStatesChanged?.Invoke();
    }
    
    public void Dispose()
    {
        _themeStates.StateChanged -= ThemeStatesOnStateChanged;
    }
}