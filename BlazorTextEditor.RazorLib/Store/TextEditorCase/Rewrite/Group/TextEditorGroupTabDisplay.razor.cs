using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

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

    private void OnClickSetActiveTextEditorViewModel()
    {
        TextEditorService.SetActiveViewModelOfGroup(
            TextEditorGroup.TextEditorGroupKey,
            TextEditorViewModelKey);
    }
}