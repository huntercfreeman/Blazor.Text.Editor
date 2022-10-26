using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public class ThemeStatesReducer
{
    [ReducerMethod]
    public static ThemeStates ReduceRegisterThemeAction(
        ThemeStates previousThemeStates,
        RegisterThemeAction registerThemeAction)
    {
        return previousThemeStates with
        {
            Themes = previousThemeStates.Themes
                .Add(registerThemeAction.Theme),
        };
    }

    [ReducerMethod]
    public static ThemeStates ReduceDisposeThemeAction(
        ThemeStates previousThemeStates,
        DisposeThemeAction disposeThemeAction)
    {
        var theme = previousThemeStates.Themes
            .FirstOrDefault(t =>
                t.ThemeKey == disposeThemeAction.ThemeKey);

        if (theme is null)
            return previousThemeStates;

        return previousThemeStates with
        {
            Themes = previousThemeStates.Themes
                .Remove(theme),
        };
    }
}