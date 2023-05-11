namespace BlazorTextEditor.RazorLib.Find;

public record TextEditorFindProviderKey(Guid Guid)
{
    public static readonly TextEditorFindProviderKey Empty = new TextEditorFindProviderKey(Guid.Empty);

    public static TextEditorFindProviderKey NewFindProviderKey()
    {
        return new TextEditorFindProviderKey(Guid.NewGuid());
    }
}
