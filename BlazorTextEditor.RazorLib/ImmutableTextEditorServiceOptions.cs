using System.Collections.Immutable;
using BlazorCommon.RazorLib.Theme;

namespace BlazorTextEditor.RazorLib;

public class ImmutableTextEditorServiceOptions : ITextEditorServiceOptions
{
    public ImmutableTextEditorServiceOptions(
        TextEditorServiceOptions textEditorServiceOptions)
    {
        InitializeFluxor = textEditorServiceOptions.InitializeFluxor;
        InitialThemeKey = textEditorServiceOptions.InitialThemeKey;
        CustomThemeRecords = textEditorServiceOptions.CustomThemeRecords;
        InitialTheme = textEditorServiceOptions.InitialTheme;
    }

    public bool InitializeFluxor { get; }
    public ThemeKey? InitialThemeKey { get; }
    public ImmutableArray<ThemeRecord>? CustomThemeRecords { get; }
    public ThemeRecord InitialTheme { get; }
}