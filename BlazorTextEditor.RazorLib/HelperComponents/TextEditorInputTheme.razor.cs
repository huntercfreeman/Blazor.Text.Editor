using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputTheme : FluxorComponent
{
    [Inject]
    private IState<ThemeStates> ThemeStatesWrap { get; set; } = null!;
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
        var themes = ThemeStatesWrap.Value.Themes;
        
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
            TextEditorService.SetTheme(ThemeFacts.Unset);
        }
    }
}