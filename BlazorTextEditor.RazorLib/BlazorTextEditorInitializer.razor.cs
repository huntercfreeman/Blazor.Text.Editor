using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Store.Options;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial class BlazorTextEditorInitializer : ComponentBase
{
    [Inject]
    private BlazorTextEditorOptions BlazorTextEditorOptions { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IThemeRecordsCollectionService ThemeRecordsCollectionService { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (BlazorTextEditorOptions.CustomThemeRecords is not null)
        {
            foreach (var themeRecord in BlazorTextEditorOptions.CustomThemeRecords)
            {
                Dispatcher.Dispatch(
                    new ThemeRecordsCollection.RegisterAction(
                        themeRecord));
            }
        }

        var initialThemeRecord = ThemeRecordsCollectionService.ThemeRecordsCollectionWrap.Value.ThemeRecordsList
            .FirstOrDefault(x => x.ThemeKey == BlazorTextEditorOptions.InitialThemeKey);

        if (initialThemeRecord is not null)
        {
            Dispatcher.Dispatch(
                new TextEditorOptionsState.SetThemeAction(
                    initialThemeRecord));
        }
        
        base.OnInitialized();
    }
}