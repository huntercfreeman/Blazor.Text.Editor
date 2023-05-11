using BlazorTextEditor.RazorLib.Store.Find;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Find;

public partial class TextEditorFindDisplay : FluxorComponent
{
    [Inject]
    private IState<TextEditorFindProvidersCollection> TextEditorFindProvidersCollectionWrap { get; set; } = null!;
}