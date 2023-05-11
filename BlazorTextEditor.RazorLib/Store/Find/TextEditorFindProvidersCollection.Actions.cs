using BlazorTextEditor.RazorLib.Find;

namespace BlazorTextEditor.RazorLib.Store.Find;

public partial class TextEditorFindProvidersCollection
{
    public record RegisterAction(
        ITextEditorFindProvider FindProvider);

    public record DisposeAction(
        TextEditorFindProviderKey FindProviderKey);

    public record SetActiveFindProviderAction(
        TextEditorFindProviderKey FindProviderKey);
}
