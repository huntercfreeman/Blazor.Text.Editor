using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Model;

public partial class TextEditorModelDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorModelsCollection?> TextEditorModelWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorModelKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorModel"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorModelKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorModelKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public TextEditorModelKey TextEditorModelKey { get; set; } = null!;

    private TextEditorModelKey _previousTextEditorModelKey;
    private TextEditorRenderStateKey _previousTextEditorRenderStateKey;

    protected override void OnInitialized()
    {
        TextEditorModelWrap.StateChanged += TextEditorModelWrapOnStateChanged;

        base.OnInitialized();
    }

    private void TextEditorModelWrapOnStateChanged(object? sender, EventArgs e)
    {
        var textEditorModelsCollection = TextEditorModelWrap.Value;
        
        if (textEditorModelsCollection is null)
            return;
    
        if (!textEditorModelsCollection.ModelsMap.TryGetValue(
                TextEditorModelKey,
                out var textEditorModel))
        {
            return;
        }
    
        if (textEditorModel.TextEditorRenderStateKey != _previousTextEditorRenderStateKey)
        {
            _previousTextEditorRenderStateKey = textEditorModel.TextEditorRenderStateKey;
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        TextEditorModelWrap.StateChanged -= TextEditorModelWrapOnStateChanged;
    }
}