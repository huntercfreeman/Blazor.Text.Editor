namespace BlazorTextEditor.RazorLib.Keyboard;

public record KeyboardKeyCode(string Key, string Code)
{
    public bool IsLower { get; init; }
}