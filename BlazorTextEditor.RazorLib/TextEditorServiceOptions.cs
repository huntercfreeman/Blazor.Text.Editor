using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;

namespace BlazorTextEditor.RazorLib;

public class TextEditorServiceOptions : ITextEditorServiceOptions
{
    public bool InitializeFluxor { get; set; } = true;
    public Theme InitialTheme { get; set; } = ThemeFacts.Unset;
    /// <summary>
    /// Default value if left null is: <see cref="JavaScriptInteropClipboardProvider"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own or use the remaining pre-made options.
    /// <br/>
    /// <see cref="InMemoryClipboardProvider"/>
    /// </summary>
    public Func<IServiceProvider, IClipboardProvider>? ClipboardProviderFactory { get; set; }
    /// <summary>
    /// Default value if left null is: <see cref="LocalStorageProvider"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own or use the remaining pre-made options.
    /// <br/>
    /// <see cref="DoNothingStorageProvider"/>
    /// </summary>
    public Func<IServiceProvider, IStorageProvider>? StorageProviderFactory { get; set; }
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteService"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteService>? AutocompleteServiceFactory { get; set; }
    /// <summary>
    /// Default value if left null is: <see cref="AutocompleteIndexer"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own.
    /// </summary>
    public Func<IServiceProvider, IAutocompleteIndexer>? AutocompleteIndexerFactory { get; set; }
}