using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial class BlazorTextEditorInitializer : ComponentBase
{
    [Inject]
    private ITextEditorServiceOptions TextEditorServiceOptions { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void OnInitialized()
    {
        Dispatcher.Dispatch(new TextEditorSetThemeAction(
            TextEditorServiceOptions.InitialTheme));
        
        base.OnInitialized();
    }
}