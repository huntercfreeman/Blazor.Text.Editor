using BlazorTextEditor.RazorLib.Clipboard;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public class ClipboardProviderDefault : IClipboardProvider
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardProviderDefault(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>(
                "blazorTextEditor.readClipboard");
        }
        catch (TaskCanceledException e)
        {
            return string.Empty;
        }
    }

    public async Task SetClipboard(string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.setClipboard",
                value);
        }
        catch (TaskCanceledException e)
        {
        }
    }
}
