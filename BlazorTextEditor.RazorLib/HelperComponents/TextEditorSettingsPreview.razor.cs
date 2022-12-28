using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorSettingsPreview : FluxorComponent
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
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

    public static readonly TextEditorKey SettingsPreviewTextEditorKey = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorViewModelKey SettingsPreviewTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterPlainTextEditor(
            SettingsPreviewTextEditorKey,
            "SettingsPreviewTextEditorKey",
            DateTime.UtcNow,
            "Settings Preview",
            "Preview settings here");
        
        TextEditorService.RegisterViewModel(
            SettingsPreviewTextEditorViewModelKey,
            SettingsPreviewTextEditorKey);
        
        base.OnInitialized();
    }
}