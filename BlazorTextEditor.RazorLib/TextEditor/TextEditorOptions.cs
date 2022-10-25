using BlazorTextEditor.RazorLib.Store.ThemeCase;

namespace BlazorTextEditor.RazorLib.MoveThese;

/// <summary>
/// Any property on <see cref="TextEditorServiceOptions"/> will be equal to
/// the 
/// </summary>
public record TextEditorOptions(
    int? FontSizeInPixels,
    Theme? Theme,
    bool? ShowWhitespace,
    bool? ShowNewlines)
{
    public static TextEditorOptions UnsetTextEditorOptions()
    {
        return new(
            null, 
            null,
            false,
            false);
    }
}