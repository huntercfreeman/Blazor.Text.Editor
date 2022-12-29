using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Keymap;

namespace BlazorTextEditor.RazorLib.TextEditor;

/// <summary>
/// The Global instance of <see cref="TextEditorOptions"/> is not to have any null
/// values.
/// <br/><br/>
/// TODO: Marking properties nullable and then mysteriously deciding the global instance of the options is not to have nulls is confusing and needs changed. The nullable properties are for TextEditors that have specific values they want to override the global options with. 
/// </summary>
public record TextEditorOptions(
    int? FontSizeInPixels,
    ThemeRecord? Theme,
    bool? ShowWhitespace,
    bool? ShowNewlines,
    int? HeightInPixels,
    double? CursorWidthInPixels,
    KeymapDefinition? KeymapDefinition)
{
    public static TextEditorOptions UnsetTextEditorOptions()
    {
        return new TextEditorOptions(
            null,
            null,
            false,
            false,
            null,
            2.5,
            KeymapFacts.DefaultKeymapDefinition);
    }
}