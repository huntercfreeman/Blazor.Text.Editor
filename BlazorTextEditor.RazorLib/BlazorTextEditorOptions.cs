using System.Collections.Immutable;
using BlazorCommon.RazorLib;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Autocomplete;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public record BlazorTextEditorOptions
{
    public bool InitializeFluxor { get; init; } = true;
    public ThemeKey? InitialThemeKey { get; init; }
    public ImmutableArray<ThemeRecord>? CustomThemeRecords { get; init; } =
        BlazorTextEditorCustomThemeFacts.AllCustomThemes;
    public ThemeRecord InitialTheme { get; init; } = ThemeFacts.VisualStudioDarkThemeClone;
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteService"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteService> AutocompleteServiceFactory { get; init; } = serviceProvider =>
        new AutocompleteService(serviceProvider.GetRequiredService<IAutocompleteIndexer>());
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteIndexer"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteIndexer> AutocompleteIndexerFactory { get; init; } = serviceProvider =>
        new AutocompleteIndexer(serviceProvider.GetRequiredService<ITextEditorService>());

    /// <summary>
    /// Provide null if one wishes to not have BlazorCommonServices initialized from within Blazor.Text.Editor
    /// but instead, one wishes to initialize BlazorCommonServices themselves manually.
    /// </summary>
    public BlazorCommonOptions? BlazorCommonOptions { get; init; } = new();
}