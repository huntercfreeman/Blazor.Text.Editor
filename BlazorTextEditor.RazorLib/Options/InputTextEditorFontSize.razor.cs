using BlazorCommon.RazorLib.Options;
using BlazorCommon.RazorLib.Store.ApplicationOptions;
using BlazorTextEditor.RazorLib.Store.Options;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class InputTextEditorFontSize : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    public int FontSizeInPixels
    {
        get => TextEditorService.OptionsWrap.Value.Options.CommonOptions?.FontSizeInPixels ??
               TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;
        set
        {
            if (value < TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS)
                value = TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS;
            
            TextEditorService.Options.SetFontSize(value);
        }
    }

    protected override void OnInitialized()
    {
        TextEditorService.OptionsWrap.StateChanged += OptionsWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private async void OptionsWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TextEditorService.OptionsWrap.StateChanged -= OptionsWrapOnStateChanged;
    }
}