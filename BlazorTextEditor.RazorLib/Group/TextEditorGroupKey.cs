namespace BlazorTextEditor.RazorLib.Group;

/// <summary>
/// <see cref="TextEditorGroupKey"/> is used to uniquely 
/// identify a <see cref="TextEditorGroup"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorGroup"/>
/// will take a <see cref="TextEditorGroupKey"/> as a parameter.
/// </summary>
public record TextEditorGroupKey(Guid Guid)
{
    public static readonly TextEditorGroupKey Empty = new TextEditorGroupKey(Guid.Empty);

    public static TextEditorGroupKey NewTextEditorGroupKey()
    {
        return new TextEditorGroupKey(Guid.NewGuid());
    }
}