using BlazorTextEditor.RazorLib.Store.Find;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Find;

public partial class TextEditorFindDisplay : FluxorComponent
{
    [Inject]
    private IState<TextEditorFindProviderState> FindProviderState { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string SearchQuery
    {
        get => FindProviderState.Value.SearchQuery;
        set
        {
            if (value is not null)
            {
                Dispatcher.Dispatch(
                    new TextEditorFindProviderState.SetSearchQueryAction(
                        value));
            }
        }
    }
}