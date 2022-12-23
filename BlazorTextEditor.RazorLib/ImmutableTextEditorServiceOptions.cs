using BlazorALaCarte.Shared.Theme;

namespace BlazorTextEditor.RazorLib;

public class ImmutableTextEditorServiceOptions : ITextEditorServiceOptions
{
    public ImmutableTextEditorServiceOptions(
        TextEditorServiceOptions textEditorServiceOptions)
    {
        InitializeFluxor = textEditorServiceOptions.InitializeFluxor;
        InitialTheme = textEditorServiceOptions.InitialTheme;
    }

    public bool InitializeFluxor { get; }
    public ThemeRecord InitialTheme { get; set; }
}