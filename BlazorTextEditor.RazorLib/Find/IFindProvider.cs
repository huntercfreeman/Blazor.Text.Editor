namespace BlazorTextEditor.RazorLib.Find;

public interface ITextEditorFindProvider
{
    public TextEditorFindProviderKey FindProviderKey { get; }
    public Type IconComponentRendererType { get; }
    public string DisplayName { get; }
}
