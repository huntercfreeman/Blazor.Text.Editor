namespace BlazorTextEditor.RazorLib;

public class ImmutableTextEditorServiceOptions : ITextEditorServiceOptions
{
    public ImmutableTextEditorServiceOptions(TextEditorServiceOptions textEditorServiceOptions)
    {
        InitializeFluxor = textEditorServiceOptions.InitializeFluxor;
        UseLocalStorageForSettings = textEditorServiceOptions.UseLocalStorageForSettings;
    }

    public bool InitializeFluxor { get; }
    public bool UseLocalStorageForSettings { get; set; }
}