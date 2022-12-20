namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Model;

/// <summary>
/// <see cref="TextEditorModelKey"/> is used to uniquely 
/// identify a <see cref="TextEditorModel"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorModel"/>
/// will take a <see cref="TextEditorModelKey"/> as a parameter.
/// </summary>
public record TextEditorModelKey(Guid Guid)
{
    public static readonly TextEditorModelKey Empty = new TextEditorModelKey(Guid.Empty);

    public static TextEditorModelKey NewTextEditorModelKey()
    {
        return new TextEditorModelKey(Guid.NewGuid());
    }
}