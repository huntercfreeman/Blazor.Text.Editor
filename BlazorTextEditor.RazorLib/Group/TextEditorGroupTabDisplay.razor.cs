using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Group;

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

    private string IsActiveCssClass => TextEditorGroup.ActiveViewModelKey == TextEditorViewModelKey
        ? "bcrl_active"
        : string.Empty;

    private void OnClickSetActiveTextEditorViewModel()
    {
        TextEditorService.Group.SetActiveViewModel(
            TextEditorGroup.GroupKey,
            TextEditorViewModelKey);
    }
    
    private void OnMouseDown(MouseEventArgs mouseEventArgs)
    {
        if (mouseEventArgs.Button == 1)
            CloseTabOnClick();
    }

    private void CloseTabOnClick()
    {
        TextEditorService.Group.RemoveViewModel(
            TextEditorGroup.GroupKey,
            TextEditorViewModelKey);
    }
}