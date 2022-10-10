namespace BlazorTextEditor.ClassLib.TextEditor;

public record TextEditorKey(Guid Guid)
{
    public static TextEditorKey NewTextEditorKey()
    {
        return new(Guid.NewGuid());
    }
}