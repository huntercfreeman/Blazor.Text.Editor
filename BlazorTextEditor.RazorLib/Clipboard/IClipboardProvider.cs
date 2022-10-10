namespace BlazorTextEditor.RazorLib.Clipboard;

public interface IClipboardProvider
{
    public Task<string> ReadClipboard();
    public Task SetClipboard(string value);
}