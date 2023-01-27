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

    // TODO: The values written in the calc() were thrown together quickly. This needs to be done thoroughly.
    private const string TEXT_EDITOR_GROUP_DISPLAY_CSS_STYLE_STRING = "height: calc(100% - 10em);";

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

    private void CreateNewFileOnClick()
    {
        var selectedWellKnownModelKind = DemoStateContainer.SelectedWellKnownModelKind;
        var inputResourceUri = DemoStateContainer.InputResourceUri;
        var inputInitializeWithText = DemoStateContainer.InputInitializeWithText;

        var initialContent = string.Empty;
        
        if (inputInitializeWithText)
        {
            initialContent = selectedWellKnownModelKind switch
            {
                WellKnownModelKind.CSharp => TestData.CSharp.EXAMPLE_TEXT_8_LINES,
                WellKnownModelKind.Html => TestData.Html.EXAMPLE_TEXT,
                WellKnownModelKind.Css => TestData.Css.EXAMPLE_TEXT_21_LINES,
                WellKnownModelKind.Json => TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS,
                WellKnownModelKind.FSharp => TestData.FSharp.EXAMPLE_TEXT_21_LINES,
                WellKnownModelKind.Razor => TestData.Razor.EXAMPLE_TEXT,
                WellKnownModelKind.JavaScript => TestData.JavaScript.EXAMPLE_TEXT,
                WellKnownModelKind.TypeScript => TestData.TypeScript.EXAMPLE_TEXT,
                WellKnownModelKind.Plain => TestData.Plain.EXAMPLE_TEXT_25_LINES,
                _ => string.Empty
            };
        }

        var existingTextEditorModel = TextEditorService.GetTextEditorModelOrDefaultByResourceUri(
            inputResourceUri);

        if (existingTextEditorModel is not null)
        {
            HandleExistingTextEditorModel(existingTextEditorModel);
            return;
        }
        
        var newFileModelKey = TextEditorModelKey.NewTextEditorModelKey();

        TextEditorService.RegisterTemplatedTextEditorModel(
            newFileModelKey,
            selectedWellKnownModelKind,
            inputResourceUri,
            DateTime.UtcNow,
            selectedWellKnownModelKind.ToString(),
            initialContent);

        var newFileViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
        
        TextEditorService.RegisterViewModel(
            newFileViewModelKey,
            newFileModelKey);

        TextEditorService.AddViewModelToGroup(
            TextEditorDemoGroupKey,
            newFileViewModelKey);
        
        TextEditorService.SetActiveViewModelOfGroup(
            TextEditorDemoGroupKey,
            newFileViewModelKey);
    }

    private void HandleExistingTextEditorModel(TextEditorModel textEditorModel)
    {
        var listForExistingTextEditorViewModels = TextEditorService
            .GetViewModelsForModel(textEditorModel.ModelKey);

        var viewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

        if (listForExistingTextEditorViewModels.Any())
        {
            var existingTextEditorViewModel = listForExistingTextEditorViewModels.First();
            
            viewModelKey = existingTextEditorViewModel.ViewModelKey;
        }
        else
        {
            TextEditorService.RegisterViewModel(
                viewModelKey,
                textEditorModel.ModelKey);
        }
        
        TextEditorService.AddViewModelToGroup(
            TextEditorDemoGroupKey,
            viewModelKey);
        
        TextEditorService.SetActiveViewModelOfGroup(
            TextEditorDemoGroupKey,
            viewModelKey);
    }
}