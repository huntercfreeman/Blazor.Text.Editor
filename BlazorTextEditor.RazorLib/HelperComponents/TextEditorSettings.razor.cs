using BlazorTextEditor.RazorLib.Autocomplete;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorSettings : ComponentBase
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
}