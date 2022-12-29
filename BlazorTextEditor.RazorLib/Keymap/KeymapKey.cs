namespace BlazorTextEditor.RazorLib.Keymap;

public record KeymapKey(Guid Guid)
{
    /// <summary>
    /// Used instead of a null reference
    /// </summary>
    public static KeymapKey Empty { get; } = new(Guid.Empty);

    public static KeymapKey NewKeymapKey()
    {
        return new KeymapKey(Guid.NewGuid());
    }
}