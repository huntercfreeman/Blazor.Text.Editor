using BlazorTextEditor.RazorLib.Store.TextEditorCase;
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
/// Blazor component and a <see cref="TextEditorModel"/>
/// </summary>
public class TextEditorView : FluxorComponent
{
    // TODO: Do not rerender so much too many things are touched by the [Inject] in this file
    //
    // [Inject]
    // protected IStateSelection<TextEditorStates, TextEditorModel?> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    protected IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;
    
    public TextEditorModel? MutableReferenceToTextEditor => TextEditorService
        .GetTextEditorModelFromViewModelKey(TextEditorViewModelKey);
    
    public TextEditorViewModel? ReplaceableTextEditorViewModel => TextEditorViewModelsCollectionWrap.Value.ViewModelsList
        .FirstOrDefault(x => 
            x.TextEditorViewModelKey == TextEditorViewModelKey);

    private TextEditorRenderStateKey _previousViewModelRenderStateKey = TextEditorRenderStateKey.Empty;
}