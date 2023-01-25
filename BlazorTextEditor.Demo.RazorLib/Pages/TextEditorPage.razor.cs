using BlazorTextEditor.RazorLib.Model;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class TextEditorPage : ComponentBase
{
    [Inject]
    private IDemoStateContainer DemoStateContainer { get; set; } = null!;

    private const string INPUT_INITIALIZE_WITH_TEXT_ELEMENT_ID = "bted_input-initialize-with-text";
    private const string INPUT_RESOURCE_URI_ELEMENT_ID = "bted_input-resource-uri";
    private const string INPUT_WELL_KNOWN_MODEL_KIND_ELEMENT_ID = "bted_input-well-known-model-kind";
    
    private bool CheckIsSelected(string wellKnownModelKind)
    {
        return DemoStateContainer.SelectedWellKnownModelKind.ToString() ==
               wellKnownModelKind;
    }

    private void OnSelectedWellKnownModelKindChanged(ChangeEventArgs changeEventArgs)
    {
        if (changeEventArgs.Value is not string changeEventArgsAsString)
            return;

        var selectedWellKnownModelKind = Enum.Parse<WellKnownModelKind>(changeEventArgsAsString);

        DemoStateContainer.SelectedWellKnownModelKind = selectedWellKnownModelKind;
    }
}