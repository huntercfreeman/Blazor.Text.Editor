namespace BlazorTextEditor.RazorLib.TextEditor;

public record TextEditorKey(Guid Guid)
{
    public static TextEditorKey Empty { get; } = new(Guid.Empty);

    public static TextEditorKey NewTextEditorKey()
    {
        return new TextEditorKey(Guid.NewGuid());
    }
}