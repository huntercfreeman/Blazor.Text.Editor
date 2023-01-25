using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.Demo.RazorLib.TextEditorDemos;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class TextEditorPage : ComponentBase
{
    [Inject]
    private IDemoStateContainer DemoStateContainer { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private const string INPUT_INITIALIZE_WITH_TEXT_ELEMENT_ID = "bted_input-initialize-with-text";
    private const string INPUT_RESOURCE_URI_ELEMENT_ID = "bted_input-resource-uri";
    private const string INPUT_WELL_KNOWN_MODEL_KIND_ELEMENT_ID = "bted_input-well-known-model-kind";

    // TODO: The 17em written in the calc() was thrown together quickly. This needs to be done thoroughly.
    private const string TEXT_EDITOR_GROUP_DISPLAY_CSS_STYLE_STRING = "height: calc(100% - 17em);";

    private static readonly TextEditorGroupKey TextEditorDemoGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();

    private static readonly TextEditorViewModelKey CSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    private static readonly TextEditorViewModelKey RazorTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterGroup(TextEditorDemoGroupKey);

        InitializeSampleCSharpEditor();
        InitializeSampleRazorEditor();

        base.OnInitialized();
    }

    private void InitializeSampleCSharpEditor()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.CSharp.CSharpTextEditorModelKey,
            WellKnownModelKind.CSharp,
            nameof(CSharpDemo),
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        TextEditorService.RegisterViewModel(
            CSharpTextEditorViewModelKey,
            TextEditorFacts.CSharp.CSharpTextEditorModelKey);
        
        TextEditorService.AddViewModelToGroup(
            TextEditorDemoGroupKey,
            CSharpTextEditorViewModelKey);
    }

    private void InitializeSampleRazorEditor()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.Razor.RazorTextEditorModelKey,
            WellKnownModelKind.Razor,
            nameof(RazorDemo),
            DateTime.UtcNow,
            "Razor",
            TestData.Razor.EXAMPLE_TEXT);

        TextEditorService.RegisterViewModel(
            RazorTextEditorViewModelKey,
            TextEditorFacts.Razor.RazorTextEditorModelKey);

        TextEditorService.AddViewModelToGroup(
            TextEditorDemoGroupKey,
            RazorTextEditorViewModelKey);
    }

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