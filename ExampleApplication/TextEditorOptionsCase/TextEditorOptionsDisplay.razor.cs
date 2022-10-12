using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.TextEditorOptionsCase;

public partial class TextEditorOptionsDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IThemeService ThemeService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorOptions TextEditorOptions { get; set; } = null!;

    private const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    
    private int _fontSizeInPixels;
    

    private int FontSizeInPixels
    {
        get => _fontSizeInPixels;
        set
        {
            if (value > MINIMUM_FONT_SIZE_IN_PIXELS)
                _fontSizeInPixels = value;
            else
                _fontSizeInPixels = MINIMUM_FONT_SIZE_IN_PIXELS;
            
            TextEditorService.SetFontSize(_fontSizeInPixels);
        }
    }

    protected override void OnInitialized()
    {
        _fontSizeInPixels = TextEditorOptions.FontSizeInPixels ?? MINIMUM_FONT_SIZE_IN_PIXELS;
        
        
        base.OnInitialized();
    }

    private void SelectThemeOnChange(ChangeEventArgs changeEventArgs)
    {
        var themeKeyString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Guid.TryParse(themeKeyString, out var themeKey))
        {
            var theme = ThemeService.ThemeStates.Themes
                .SingleOrDefault(t => t.ThemeKey.Guid == themeKey);

            if (theme is not null)
            {
                TextEditorService.SetTheme(theme);
            }
        }
    }

    private void ShowWhitespaceToggleOnChange(ChangeEventArgs changeEventArgs)
    {
        TextEditorService.SetShowWhitespace((bool)(changeEventArgs.Value ?? false));
    }

    private void ShowNewlinesToggleOnChange(ChangeEventArgs changeEventArgs)
    {
        TextEditorService.SetShowNewlines((bool)(changeEventArgs.Value ?? false));
    }
}