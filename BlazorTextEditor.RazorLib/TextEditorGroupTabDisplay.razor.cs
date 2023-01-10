using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
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
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    
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

    private void CloseTabOnClick()
    {
        TextEditorService.RemoveViewModelFromGroup(
            TextEditorGroup.TextEditorGroupKey,
            TextEditorViewModelKey);
    }
}