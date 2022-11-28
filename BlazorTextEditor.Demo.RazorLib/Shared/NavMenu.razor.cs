using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Shared;

public partial class NavMenu : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}