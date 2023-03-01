using System.Collections.Immutable;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Autocomplete;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public class TextEditorServiceOptions : ITextEditorServiceOptions
{
    public bool InitializeFluxor { get; set; } = true;
    public ThemeKey? InitialThemeKey { get; set; }
    public ImmutableArray<ThemeRecord>? InitialThemeRecords { get; set; }
    public ThemeRecord InitialTheme { get; set; } = ThemeFacts.VisualStudioDarkThemeClone;
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteService"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteService> AutocompleteServiceFactory { get; set; } = serviceProvider =>
        new AutocompleteService(serviceProvider.GetRequiredService<IAutocompleteIndexer>());
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteIndexer"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteIndexer> AutocompleteIndexerFactory { get; set; } = serviceProvider =>
        new AutocompleteIndexer(serviceProvider.GetRequiredService<ITextEditorService>());
}