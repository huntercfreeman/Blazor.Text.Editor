using BlazorTextEditor.RazorLib.Find;

namespace BlazorTextEditor.RazorLib.Store.Find;

public partial class TextEditorFindProviderState
{
    public record RegisterAction(
        ITextEditorFindProvider FindProvider);

    public record DisposeAction(
        TextEditorFindProviderKey FindProviderKey);

    public record SetActiveFindProviderAction(
        TextEditorFindProviderKey FindProviderKey);
    
    public record SetSearchQueryAction(
        string SearchQuery);
}
