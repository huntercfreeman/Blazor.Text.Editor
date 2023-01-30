using BlazorALaCarte.Shared.Facts;
using BlazorALaCarte.Shared.Options;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Options;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.GlobalOptions;

/// <summary>
/// Keep the <see cref="TextEditorGlobalOptions"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorGlobalOptions
{
    public TextEditorGlobalOptions()
    {
        Options = new TextEditorOptions(
            new CommonOptions(
                20,
                ThemeFacts.VisualStudioDarkThemeClone.ThemeKey),
            false,
            false,
            null,
            2.5,
            KeymapFacts.DefaultKeymapDefinition);
    }
    
    public TextEditorOptions Options { get; set; }
}