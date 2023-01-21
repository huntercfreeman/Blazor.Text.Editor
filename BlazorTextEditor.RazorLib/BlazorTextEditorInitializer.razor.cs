using BlazorALaCarte.Shared.Services;
using BlazorALaCarte.Shared.Store.ThemeCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial class BlazorTextEditorInitializer : ComponentBase
{
    [Inject]
    private ITextEditorServiceOptions TextEditorServiceOptions { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IThemeService ThemeService { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (TextEditorServiceOptions.InitialThemeRecords is not null)
        {
            foreach (var themeRecord in TextEditorServiceOptions.InitialThemeRecords)
            {
                Dispatcher.Dispatch(
                    new ThemeState.RegisterAction(
                        themeRecord));
            }
        }

        var initialThemeRecord = ThemeService.ThemeStateWrap.Value.ThemeRecordsList
            .FirstOrDefault(x => x.ThemeKey == TextEditorServiceOptions.InitialThemeKey);

        if (initialThemeRecord is not null)
        {
            Dispatcher.Dispatch(
                new TextEditorGlobalOptions.SetThemeAction(
                    initialThemeRecord));
        }
        
        base.OnInitialized();
    }
}