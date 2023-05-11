using BlazorTextEditor.RazorLib.Store.Find;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Find.InternalComponents;

public partial class FindProviderTabDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorFindProvider TextEditorFindProvider { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsActive { get; set; }

    private string IsActiveCssClassString => IsActive
        ? "bcrl_active"
        : "";

    private void DispatchSetActiveFindProviderActionOnClick()
    {
        Dispatcher.Dispatch(
            new TextEditorFindProvidersCollection.SetActiveFindProviderAction(
                TextEditorFindProvider.FindProviderKey));
    }
}