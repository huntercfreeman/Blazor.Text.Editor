using BlazorCommon.RazorLib.Options;
using BlazorTextEditor.RazorLib.Keymap;

namespace BlazorTextEditor.RazorLib.Options;

public record TextEditorOptions(
    CommonOptions? CommonOptions,
    bool? ShowWhitespace,
    bool? ShowNewlines,
    int? TextEditorHeightInPixels,
    double? CursorWidthInPixels,
    KeymapDefinition? KeymapDefinition);