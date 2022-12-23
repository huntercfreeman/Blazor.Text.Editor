﻿using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

/// <summary>
/// <see cref="TextEditorView"/> is the
/// message broker abstraction between a
/// Blazor component and a <see cref="TextEditorBase"/>
/// </summary>
public class TextEditorView : FluxorComponent, IDisposable
{
    // TODO: Do not rerender so much too many things are touched by the [Inject] in this file
    //
    // [Inject]
    // protected IStateSelection<TextEditorStates, TextEditorBase?> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    protected IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;
    
    public TextEditorBase? MutableReferenceToTextEditor => TextEditorService
        .GetTextEditorBaseFromViewModelKey(TextEditorViewModelKey);
    public TextEditorViewModel? ReplaceableTextEditorViewModel => TextEditorViewModelsCollectionWrap.Value.ViewModelsList
        .FirstOrDefault(x => 
            x.TextEditorViewModelKey == TextEditorViewModelKey);

    private TextEditorRenderStateKey _previousViewModelRenderStateKey = TextEditorRenderStateKey.Empty;
    //private bool _disposed;

    // protected override void OnInitialized()
    // {
    //     TextEditorStatesSelection.StateChanged += TextEditorStatesSelectionOnStateChanged;
    //     TextEditorViewModelsCollectionWrap.StateChanged += TextEditorViewModelsCollectionWrapOnStateChanged;
    //     
    //     TextEditorStatesWrap
    //         .Select(textEditorStates => textEditorStates.TextEditorList
    //             .SingleOrDefault(x => 
    //                 x.Key == (ReplaceableTextEditorViewModel?.TextEditorKey ?? TextEditorKey.Empty)));
    //     
    //     base.OnInitialized();
    // }

    // private void TextEditorStatesSelectionOnStateChanged(object? sender, EventArgs e)
    // {
    //     InvokeAsync(StateHasChanged);
    // }
    //
    // private void TextEditorViewModelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    // {
    //     var viewModel = ReplaceableTextEditorViewModel;
    //
    //     var currentViewModelRenderStateKey = viewModel?.TextEditorRenderStateKey ??
    //                                          TextEditorRenderStateKey.Empty;
    //     
    //     if (_previousViewModelRenderStateKey != currentViewModelRenderStateKey)
    //     {
    //         _previousViewModelRenderStateKey = currentViewModelRenderStateKey;
    //         InvokeAsync(StateHasChanged);
    //     }
    // }
    
    // TODO: Commenting out the event logic going to try FluxorComponent
    //
    // protected virtual void Dispose(bool disposing)
    // {
    //     if (_disposed)
    //     {
    //         return;
    //     }
    //
    //     if (disposing)
    //     {
    //         TextEditorViewModelsCollectionWrap.StateChanged -= TextEditorViewModelsCollectionWrapOnStateChanged;
    //         TextEditorStatesSelection.StateChanged -= TextEditorStatesSelectionOnStateChanged;
    //     }
    //
    //     _disposed = true;
    // }
    
    // public void Dispose()
    // {
    //     Dispose(true);
    //     GC.SuppressFinalize(this);
    // }
}