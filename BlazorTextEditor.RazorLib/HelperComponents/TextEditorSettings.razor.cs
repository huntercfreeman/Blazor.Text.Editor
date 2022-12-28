using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorSettings : FluxorComponent
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    
    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}