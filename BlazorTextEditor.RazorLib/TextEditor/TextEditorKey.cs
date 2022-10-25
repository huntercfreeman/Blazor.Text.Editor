namespace BlazorTextEditor.RazorLib.MoveThese;

public record TextEditorKey(Guid Guid)
{
    public static TextEditorKey Empty { get; } = new TextEditorKey(Guid.Empty);
    
    public static TextEditorKey NewTextEditorKey()
    {
        return new(Guid.NewGuid());
    }
}