namespace BlazorTextEditor.RazorLib;

public class ImmutableTextEditorServiceOptions : ITextEditorServiceOptions
{
    public ImmutableTextEditorServiceOptions(TextEditorServiceOptions textEditorServiceOptions)
    {
        InitializeFluxor = textEditorServiceOptions.InitializeFluxor;
    }
    
    public bool InitializeFluxor { get; }
}