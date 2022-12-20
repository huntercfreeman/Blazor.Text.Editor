using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.View;

public partial class TextEditorViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorViewsCollection?> TextEditorViewWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorViewKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorView"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorViewKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorViewKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public TextEditorViewKey TextEditorViewKey { get; set; } = null!;

    private TextEditorViewKey _previousTextEditorViewKey;
    private TextEditorRenderStateKey _previousTextEditorRenderStateKey;

    protected override void OnInitialized()
    {
        TextEditorViewWrap.StateChanged += TextEditorViewWrapOnStateChanged;

        base.OnInitialized();
    }

    private void TextEditorViewWrapOnStateChanged(object? sender, EventArgs e)
    {
        var textEditorViewsCollection = TextEditorViewWrap.Value;
        
        if (textEditorViewsCollection is null)
            return;
    
        if (!textEditorViewsCollection.ViewsMap.TryGetValue(
                TextEditorViewKey,
                out var textEditorView))
        {
            return;
        }
    
        if (textEditorView.TextEditorRenderStateKey != _previousTextEditorRenderStateKey)
        {
            _previousTextEditorRenderStateKey = textEditorView.TextEditorRenderStateKey;
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        TextEditorViewWrap.StateChanged -= TextEditorViewWrapOnStateChanged;
    }
}