namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public record ThemeKey(Guid Guid)
{
    public static ThemeKey Empty { get; } = new(Guid.Empty);
    
    public static ThemeKey NewThemeKey()
    {
        return new ThemeKey(Guid.NewGuid());
    }
}