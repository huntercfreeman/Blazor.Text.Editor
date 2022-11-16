using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Clipboard;

public class JavaScriptInteropClipboardProvider : IClipboardProvider
{
    private readonly IJSRuntime _jsRuntime;

    public JavaScriptInteropClipboardProvider(IJSRuntime jsRuntime)
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
        catch (TaskCanceledException)
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
        catch (TaskCanceledException)
        {
        }
    }
}