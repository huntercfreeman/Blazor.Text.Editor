namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public class DoNothingStorageProvider : IStorageProvider
{
    public ValueTask SetValue(string key, object? value)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<object?> GetValue(string key)
    {
        return new ValueTask<object?>();
    }
}