namespace BlazorTextEditor.RazorLib.TextEditor;

public record TextEditorKey(Guid Guid)
{
    /// <summary>
    /// Used instead of a null reference
    /// </summary>
    public static TextEditorKey Empty { get; } = new(Guid.Empty);

    public static TextEditorKey NewTextEditorKey()
    {
        return new TextEditorKey(Guid.NewGuid());
    }
}