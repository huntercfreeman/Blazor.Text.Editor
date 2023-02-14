namespace BlazorTextEditor.RazorLib.Decoration;

/// <summary>
/// <see cref="TextEditorPresentationKey"/> is used to uniquely 
/// identify a <see cref="TextEditorPresentation"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorPresentation"/>
/// will take a <see cref="TextEditorPresentationKey"/> as a parameter.
/// </summary>
public record TextEditorPresentationKey(Guid Guid)
{
    public static readonly TextEditorPresentationKey Empty = new TextEditorPresentationKey(Guid.Empty);

    public static TextEditorPresentationKey NewTextEditorPresentationKey()
    {
        return new TextEditorPresentationKey(Guid.NewGuid());
    }
}