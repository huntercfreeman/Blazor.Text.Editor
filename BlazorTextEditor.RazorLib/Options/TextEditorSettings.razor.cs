using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Store.Model;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class TextEditorSettings : FluxorComponent
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    
    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}