namespace BlazorTextEditor.RazorLib.Diff;

/// <summary>
/// <see cref="TextEditorDiffKey"/> is used to uniquely 
/// identify a <see cref="TextEditorDiff"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorDiff"/>
/// will take a <see cref="TextEditorDiffKey"/> as a parameter.
/// </summary>
public record TextEditorDiffKey(Guid Guid)
{
    public static readonly TextEditorDiffKey Empty = new TextEditorDiffKey(Guid.Empty);

    public static TextEditorDiffKey NewTextEditorDiffKey()
    {
        return new TextEditorDiffKey(Guid.NewGuid());
    }
}