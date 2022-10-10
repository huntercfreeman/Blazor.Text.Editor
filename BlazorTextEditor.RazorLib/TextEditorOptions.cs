using BlazorTextEditor.RazorLib.Clipboard;

namespace BlazorTextEditor.RazorLib;

public class TextEditorOptions : ITextEditorOptions
{
    public bool InitializeFluxor { get; set; } = true;
    /// <summary>
    /// A default clipboard provider will be provided that invokes
    /// the JsInterop located in the Razor Lib's blazorTextEditor.js file
    /// however, one can override the clipboard provider here.
    /// </summary>
    public Func<IServiceProvider, IClipboardProvider>? ClipboardProviderFactory { get; set; }
}