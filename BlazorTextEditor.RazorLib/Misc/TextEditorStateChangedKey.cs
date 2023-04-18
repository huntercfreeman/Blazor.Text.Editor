namespace BlazorTextEditor.RazorLib.Misc;

/// <summary>
/// Used to re-render the UI.
/// This middle man to re-render the UI is necessary otherwise there would be an infinite render loop
/// because OnAfterRenderAsync modifies the ViewModel at times for an example.
/// </summary>
public record TextEditorStateChangedKey(Guid Guid)
{
    public static readonly TextEditorStateChangedKey Empty = new TextEditorStateChangedKey(Guid.Empty);

    public static TextEditorStateChangedKey NewTextEditorStateChangedKey()
    {
        return new TextEditorStateChangedKey(Guid.NewGuid());
    }
}