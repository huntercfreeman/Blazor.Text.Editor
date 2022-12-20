namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.View;

/// <summary>
/// <see cref="TextEditorViewKey"/> is used to uniquely 
/// identify a <see cref="TextEditorView"/>.
/// <br/><br/>
/// When interacting with the <see cref="ITextEditorService"/> it is
/// common that a method regarding a <see cref="TextEditorView"/>
/// will take a <see cref="TextEditorViewKey"/> as a parameter.
/// </summary>
public record TextEditorViewKey(Guid Guid)
{
    public static readonly TextEditorViewKey Empty = new TextEditorViewKey(Guid.Empty);

    public static TextEditorViewKey NewTextEditorViewKey()
    {
        return new TextEditorViewKey(Guid.NewGuid());
    }
}