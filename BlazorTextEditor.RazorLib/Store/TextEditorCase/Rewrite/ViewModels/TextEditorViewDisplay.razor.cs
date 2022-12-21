using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

public partial class TextEditorViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorViewModelsCollection?> TextEditorViewsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorStates?> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorViewModelKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorViewModel"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorViewModelKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorViewModelKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;

    private TextEditorViewModelKey _previousTextEditorViewModelKey;
    private TextEditorRenderStateKey _previousTextEditorRenderStateKey;

    protected override void OnInitialized()
    {
        TextEditorViewsCollectionWrap.StateChanged += TextEditorViewWrapOnStateChanged;

        base.OnInitialized();
    }

    private void TextEditorViewWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
        
        var textEditorViewsCollection = TextEditorViewsCollectionWrap.Value;
        
        if (textEditorViewsCollection is null)
            return;

        var textEditorViewModel = textEditorViewsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == TextEditorViewModelKey);

        if (textEditorViewModel is null)
            return;
        
        if (textEditorViewModel.TextEditorRenderStateKey != _previousTextEditorRenderStateKey)
        {
            _previousTextEditorRenderStateKey = textEditorViewModel.TextEditorRenderStateKey;
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        TextEditorViewsCollectionWrap.StateChanged -= TextEditorViewWrapOnStateChanged;
    }
}