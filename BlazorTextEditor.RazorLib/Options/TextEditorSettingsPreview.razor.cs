using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class TextEditorSettingsPreview : FluxorComponent
{
    [Inject]
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string PreviewElementCssClassString { get; set; } = string.Empty;

    public static readonly TextEditorModelKey SettingsPreviewTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    public static readonly TextEditorViewModelKey SettingsPreviewTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            SettingsPreviewTextEditorModelKey,
            WellKnownModelKind.Plain,
            "SettingsPreviewTextEditorModelKey",
            DateTime.UtcNow,
            "Settings Preview",
            "Preview settings here");
        
        TextEditorService.ViewModelRegister(
            SettingsPreviewTextEditorViewModelKey,
            SettingsPreviewTextEditorModelKey);
        
        base.OnInitialized();
    }
}