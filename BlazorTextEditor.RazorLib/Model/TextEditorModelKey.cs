namespace BlazorTextEditor.RazorLib.Model;

public record TextEditorModelKey(Guid Guid)
{
    /// <summary>
    /// Used instead of a null reference
    /// </summary>
    public static TextEditorModelKey Empty { get; } = new(Guid.Empty);

    public static TextEditorModelKey NewTextEditorModelKey()
    {
        return new TextEditorModelKey(Guid.NewGuid());
    }
}