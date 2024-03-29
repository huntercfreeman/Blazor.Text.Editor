using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class TextEditorSettingsPreview : ComponentBase
{
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
        TextEditorService.Model.RegisterTemplated(
            SettingsPreviewTextEditorModelKey,
            WellKnownModelKind.Plain,
            "SettingsPreviewTextEditorModelKey",
            DateTime.UtcNow,
            "Settings Preview",
            "Preview settings here");
        
        TextEditorService.ViewModel.Register(
            SettingsPreviewTextEditorViewModelKey,
            SettingsPreviewTextEditorModelKey);
        
        base.OnInitialized();
    }
}