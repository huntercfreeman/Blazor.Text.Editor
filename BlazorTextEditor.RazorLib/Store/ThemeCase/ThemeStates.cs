using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

[FeatureState]
public record ThemeStates(ImmutableArray<Theme> Themes)
{
    public ThemeStates() : this(ImmutableArray<Theme>.Empty)
    {
        Themes = Themes.AddRange(new[]
        {
            ThemeFacts.Unset,
            ThemeFacts.VisualStudioDarkClone,
            ThemeFacts.VisualStudioLightClone,
        });
    }
}