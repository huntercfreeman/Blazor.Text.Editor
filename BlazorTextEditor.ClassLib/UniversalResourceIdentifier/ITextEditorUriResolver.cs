namespace BlazorTextEditor.ClassLib.UniversalResourceIdentifier;

public interface ITextEditorUriResolver
{
    public Task<string> ReadTextAsync(ITextEditorUri textEditorUri);
    public Task WriteTextAsync(ITextEditorUri textEditorUri, string content);
}