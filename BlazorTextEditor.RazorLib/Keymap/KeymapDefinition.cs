using System.Text.Json.Serialization;

namespace BlazorTextEditor.RazorLib.Keymap;

public record KeymapDefinition(
    KeymapKey KeymapKey,
    string DisplayName,
    [property: JsonIgnore] ITextEditorKeymap Keymap);