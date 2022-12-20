namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;

/// <summary>
/// <see cref="TextEditorRenderStateKey"/> is used to indicate
/// that the user interface is 'dirty' and needs to re-render.
/// <br/><br/>
/// A Blazor Component for example would override the 'ShouldRender' method
/// and only rerender if the <see cref="TextEditorRenderStateKey"/> changed
/// from the last time that component was rendered.
/// </summary>
public record TextEditorRenderStateKey(Guid Guid)
{
    public static readonly TextEditorRenderStateKey Empty = new TextEditorRenderStateKey(Guid.Empty);

    public static TextEditorRenderStateKey NewTextEditorRenderStateKey()
    {
        return new TextEditorRenderStateKey(Guid.NewGuid());
    }
}