using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputFontSize : FluxorComponent
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public string EmElementCssClassString { get; set; } = string.Empty;

    private const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    
    private int _textEditorFontSize;

    private int TextEditorFontSize
    {
        get => _textEditorFontSize;
        set
        {
            if (value < MINIMUM_FONT_SIZE_IN_PIXELS)
                _textEditorFontSize = MINIMUM_FONT_SIZE_IN_PIXELS;
            else
                _textEditorFontSize = value;
            
            TextEditorService.SetFontSize(_textEditorFontSize);
        }
    }

    protected override Task OnInitializedAsync()
    {
        _textEditorFontSize = TextEditorService.GlobalFontSizeInPixelsValue;
        
        return base.OnInitializedAsync();
    }
}