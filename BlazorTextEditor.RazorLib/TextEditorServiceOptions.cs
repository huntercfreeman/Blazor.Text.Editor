using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Store.StorageCase;

namespace BlazorTextEditor.RazorLib;

public class TextEditorServiceOptions : ITextEditorServiceOptions
{
    /// <summary>
    /// Default value if left null is: <see cref="JavaScriptInteropClipboardProvider"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own or use the remaining pre-made options.
    /// <br/>
    /// <see cref="InMemoryClipboardProvider"/>
    /// </summary>
    public Func<IServiceProvider, IClipboardProvider>? ClipboardProviderFactory { get; set; }
    public bool InitializeFluxor { get; set; } = true;
    /// <summary>
    /// Default value if left null is: <see cref="LocalStorageProvider"/>
    /// <br/><br/>
    /// Additionally one can override this value with their own or use the remaining pre-made options.
    /// <br/>
    /// <see cref="DoNothingStorageProvider"/>
    /// </summary>
    public Func<IServiceProvider,IStorageProvider>? StorageProviderFactory { get; set; }
}