using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorSettingsPreview : FluxorComponent
{
    [Inject]
    private IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
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

    public static readonly TextEditorModelKey SettingsPreviewTextEditorModelKey = TextEditorModelKey.NewTextEditorKey();
    public static readonly TextEditorViewModelKey SettingsPreviewTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterPlainTextEditor(
            SettingsPreviewTextEditorModelKey,
            "SettingsPreviewTextEditorKey",
            DateTime.UtcNow,
            "Settings Preview",
            "Preview settings here");
        
        TextEditorService.RegisterViewModel(
            SettingsPreviewTextEditorViewModelKey,
            SettingsPreviewTextEditorModelKey);
        
        base.OnInitialized();
    }
}