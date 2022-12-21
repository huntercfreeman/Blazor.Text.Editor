namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

/// <summary>
/// <see cref="TextEditorViewModelKey"/> is used to uniquely 
/// identify a <see cref="TextEditorViewModel"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorViewModel"/>
/// will take a <see cref="TextEditorViewModelKey"/> as a parameter.
/// </summary>
public record TextEditorViewModelKey(Guid Guid)
{
    public static readonly TextEditorViewModelKey Empty = new TextEditorViewModelKey(Guid.Empty);

    public static TextEditorViewModelKey NewTextEditorViewModelKey()
    {
        return new TextEditorViewModelKey(Guid.NewGuid());
    }
}