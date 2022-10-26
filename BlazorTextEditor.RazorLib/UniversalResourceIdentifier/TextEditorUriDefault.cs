namespace BlazorTextEditor.RazorLib.UniversalResourceIdentifier;

public class TextEditorUriDefault : ITextEditorUri
{
    public TextEditorUriDefault(string path)
    {
        Path = path;
    }

    public string Path { get; }
}