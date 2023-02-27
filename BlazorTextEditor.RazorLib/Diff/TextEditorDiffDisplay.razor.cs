﻿using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Diff;

public partial class TextEditorDiffDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorDiffsCollection> TextEditorDiffsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorDiffKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorDiffModel"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorDiffKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorDiffKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public TextEditorDiffKey TextEditorDiffKey { get; set; } = null!;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;

    private CancellationTokenSource _calculateDiffCancellationTokenSource = new();
    
    protected override void OnInitialized()
    {
        TextEditorDiffsCollectionWrap.StateChanged += TextEditorDiffWrapOnStateChanged;
        TextEditorModelsCollectionWrap.StateChanged += TextEditorModelsCollectionWrapOnStateChanged;

        TextEditorModelsCollectionWrapOnStateChanged(null, EventArgs.Empty);
        
        base.OnInitialized();
    }

    private async void TextEditorDiffWrapOnStateChanged(
        object? sender,
        EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }
    
    private void TextEditorModelsCollectionWrapOnStateChanged(
        object? sender,
        EventArgs e)
    {
        _calculateDiffCancellationTokenSource.Cancel();
        _calculateDiffCancellationTokenSource = new();
        
        var token = _calculateDiffCancellationTokenSource.Token;
        
        TextEditorService.DiffCalculate(
            TextEditorDiffKey,
            token);
    }

    public void Dispose()
    {
        TextEditorDiffsCollectionWrap.StateChanged -= TextEditorDiffWrapOnStateChanged;
        TextEditorModelsCollectionWrap.StateChanged -=TextEditorModelsCollectionWrapOnStateChanged;

        _calculateDiffCancellationTokenSource.Cancel();
    }
}