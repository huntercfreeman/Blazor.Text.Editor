using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Keymap;

namespace BlazorTextEditor.RazorLib.Store.GlobalOptions;

public partial class TextEditorGlobalOptions
{
    public record SetFontSizeAction(int FontSizeInPixels);
    public record SetCursorWidthAction(double CursorWidthInPixels);
    public record SetHeightAction(int? HeightInPixels);
    /// <summary>
    /// This is setting the TextEditor's theme specifically.
    /// This is not to be confused with the AppOptions Themes which
    /// get applied at an application level.
    /// <br/><br/>
    /// This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"
    /// </summary>
    public record SetThemeAction(ThemeRecord Theme);
    public record SetKeymapAction(KeymapDefinition KeymapDefinition);
    public record SetShowWhitespaceAction(bool ShowWhitespace);
    public record SetShowNewlinesAction(bool ShowNewlines);
}