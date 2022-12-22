using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial class TextEditorGroupTabDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorGroup TextEditorGroup { get; set; } = null!;

    private string IsActiveCssClass => TextEditorGroup.ActiveTextEditorViewModelKey == TextEditorViewModelKey
        ? "balc_active"
        : string.Empty;

    private void OnClickSetActiveTextEditorViewModel()
    {
        TextEditorService.SetActiveViewModelOfGroup(
            TextEditorGroup.TextEditorGroupKey,
            TextEditorViewModelKey);
    }
}