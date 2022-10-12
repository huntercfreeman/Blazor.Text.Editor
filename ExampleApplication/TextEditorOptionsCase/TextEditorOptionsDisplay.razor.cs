using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.TextEditorOptionsCase;

public partial class TextEditorOptionsDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

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
}