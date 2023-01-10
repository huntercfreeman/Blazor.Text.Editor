using BlazorALaCarte.Shared.Facts;
using BlazorALaCarte.Shared.Store;
using BlazorALaCarte.Shared.Store.ThemeCase;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputTheme : FluxorComponent
{
    [Inject]
    private IState<ThemeState> ThemeStatesWrap { get; set; } = null!;
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
    
    private void SelectedThemeChanged(ChangeEventArgs changeEventArgs)
    {
        var themes = ThemeStatesWrap.Value.ThemeRecordsList;
        
        var chosenThemeKeyGuidString = changeEventArgs.Value?.ToString() ?? string.Empty;

        if (Guid.TryParse(chosenThemeKeyGuidString, 
                out var chosenThemeKeyGuid))
        {
            var chosenThemeKey = new ThemeKey(chosenThemeKeyGuid);

            var foundTheme = themes.FirstOrDefault(x => x.ThemeKey == chosenThemeKey);
            
            if (foundTheme is not null)
                TextEditorService.SetTheme(foundTheme);
        }
        else
        {
            TextEditorService.SetTheme(ThemeFacts.VisualStudioDarkThemeClone);
        }
    }
}