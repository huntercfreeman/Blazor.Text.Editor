using BlazorCommon.RazorLib.Options;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Keymap;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Options;

/// <summary>
/// Keep the <see cref="TextEditorOptionsState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorOptionsState
{
    public TextEditorOptionsState()
    {
        Options = new RazorLib.Options.TextEditorOptions(
            new CommonOptions(
                DEFAULT_FONT_SIZE_IN_PIXELS,
                DEFAULT_ICON_SIZE_IN_PIXELS,
                ThemeFacts.VisualStudioDarkThemeClone.ThemeKey,
                null),
            false,
            false,
            null,
            DEFAULT_CURSOR_WIDTH_IN_PIXELS,
            KeymapFacts.DefaultKeymapDefinition,
            true);
    }
    
    public const int DEFAULT_FONT_SIZE_IN_PIXELS = 20;
    public const int DEFAULT_ICON_SIZE_IN_PIXELS = 18;
    public const double DEFAULT_CURSOR_WIDTH_IN_PIXELS = 2.5;
    
    public const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    public const int MINIMUM_ICON_SIZE_IN_PIXELS = 5;
    
    public RazorLib.Options.TextEditorOptions Options { get; set; }
}