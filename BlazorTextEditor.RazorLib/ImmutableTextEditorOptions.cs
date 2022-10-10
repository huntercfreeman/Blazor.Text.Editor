namespace BlazorTextEditor.RazorLib;

public class ImmutableTextEditorOptions : ITextEditorOptions
{
    public ImmutableTextEditorOptions(TextEditorOptions textEditorOptions)
    {
        InitializeFluxor = textEditorOptions.InitializeFluxor;
    }
    
    public bool InitializeFluxor { get; }
}