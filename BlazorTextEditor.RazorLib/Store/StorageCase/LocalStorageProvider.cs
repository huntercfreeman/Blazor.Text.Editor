using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public class LocalStorageProvider : IStorageProvider
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async ValueTask SetValue(string key, object? value)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.localStorageSetItem",
            key,
            value);
    }

    public async ValueTask<object?> GetValue(string key)
    {
        return await _jsRuntime.InvokeAsync<string>(
            "blazorTextEditor.localStorageGetItem",
            key);
    }
}