using BlazorCommon.RazorLib.Icons.Codicon;

namespace BlazorTextEditor.RazorLib.Find;

public class RegisteredViewModelsFindProvider : ITextEditorFindProvider
{
    public TextEditorFindProviderKey FindProviderKey { get; } = 
        new TextEditorFindProviderKey(Guid.Parse("8f82c804-7813-44ea-869a-f77574f2f945"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "Registered ViewModels";
}
