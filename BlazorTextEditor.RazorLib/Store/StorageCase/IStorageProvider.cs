namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public interface IStorageProvider
{
    public ValueTask SetValue(string key, object? value);
    public ValueTask<object?> GetValue(string key);
}