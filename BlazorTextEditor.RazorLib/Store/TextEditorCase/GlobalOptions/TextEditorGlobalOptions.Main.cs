using BlazorALaCarte.Shared.Facts;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;

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
            20,
            ThemeFacts.VisualStudioDarkThemeClone,
            false,
            false,
            null,
            2.5,
            KeymapFacts.DefaultKeymapDefinition);
    }
    
    public TextEditorOptions Options { get; set; }
}