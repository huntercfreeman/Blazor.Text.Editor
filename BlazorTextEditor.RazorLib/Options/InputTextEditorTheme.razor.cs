using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Store.Options;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class InputTextEditorTheme : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;
    
    private void SelectedThemeChanged(ChangeEventArgs changeEventArgs)
    {
        var themeRecordsCollection = ThemeRecordsCollectionWrap.Value.ThemeRecordsList;
        
        var chosenThemeKeyGuidString = changeEventArgs.Value?.ToString() ?? string.Empty;

        if (Guid.TryParse(chosenThemeKeyGuidString, 
                out var chosenThemeKeyGuid))
        {
            var chosenThemeKey = new ThemeKey(chosenThemeKeyGuid);

            var foundTheme = themeRecordsCollection.FirstOrDefault(x => x.ThemeKey == chosenThemeKey);
            
            if (foundTheme is not null)
                TextEditorService.Options.SetTheme(foundTheme);
        }
        else
        {
            TextEditorService.Options.SetTheme(ThemeFacts.VisualStudioDarkThemeClone);
        }
    }
}