using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

public partial class TextEditorGroupDisplay : IDisposable
{
    [Inject]
    private IState<TextEditorGroupsCollection> TextEditorGroupWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorGroupKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorGroup"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorGroupKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorGroupKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public TextEditorGroupKey TextEditorGroupKey { get; set; } = null!;

    private TextEditorGroupKey _previousTextEditorGroupKey;
    private TextEditorRenderStateKey _previousTextEditorRenderStateKey;

    protected override void OnInitialized()
    {
        TextEditorGroupWrap.StateChanged += TextEditorGroupWrapOnStateChanged;

        base.OnInitialized();
    }

    private void TextEditorGroupWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TextEditorGroupWrap.StateChanged -= TextEditorGroupWrapOnStateChanged;
    }
}