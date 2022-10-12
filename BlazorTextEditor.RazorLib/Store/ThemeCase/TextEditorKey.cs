namespace BlazorTextEditor.RazorLib.Store.ThemeCase;

public record ThemeKey(Guid Guid)
{
    public static ThemeKey NewThemeKey()
    {
        return new(Guid.NewGuid());
    }
}