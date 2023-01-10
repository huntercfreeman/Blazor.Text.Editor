using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Keymap;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;

public partial class TextEditorGlobalOptions
{
    public record SetFontSizeAction(int FontSizeInPixels);
    public record SetCursorWidthAction(double CursorWidthInPixels);
    public record SetHeightAction(int? HeightInPixels);
    public record SetThemeAction(ThemeRecord Theme);
    public record SetKeymapAction(KeymapDefinition KeymapDefinition);
    public record SetShowWhitespaceAction(bool ShowWhitespace);
    public record SetShowNewlinesAction(bool ShowNewlines);
}