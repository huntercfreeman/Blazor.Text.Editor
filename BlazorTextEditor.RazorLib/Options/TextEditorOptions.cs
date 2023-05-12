using BlazorCommon.RazorLib.Misc;
using BlazorCommon.RazorLib.Options;
using BlazorTextEditor.RazorLib.Keymap;

namespace BlazorTextEditor.RazorLib.Options;

public record TextEditorOptions(
    CommonOptions? CommonOptions,
    bool? ShowWhitespace,
    bool? ShowNewlines,
    int? TextEditorHeightInPixels,
    double? CursorWidthInPixels,
    KeymapDefinition? KeymapDefinition,
    bool UseMonospaceOptimizations)
{
    public RenderStateKey RenderStateKey { get; init; } = RenderStateKey.NewRenderStateKey();
}