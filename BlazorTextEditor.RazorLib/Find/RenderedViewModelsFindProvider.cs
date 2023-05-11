using BlazorCommon.RazorLib.Icons.Codicon;

namespace BlazorTextEditor.RazorLib.Find;

public class RenderedViewModelsFindProvider : ITextEditorFindProvider
{
    public TextEditorFindProviderKey FindProviderKey { get; } = 
        new TextEditorFindProviderKey(Guid.Parse("9bdad472-04eb-488b-88cc-e1b6e3686399"));

    public Type IconComponentRendererType { get; } = typeof(IconArrowDown);
    public string DisplayName { get; } = "Rendered ViewModels";

    public async Task SearchAsync(
        string searchQuery,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(3_000);
    }
}
