using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;

namespace BlazorTextEditor.RazorLib.ViewModel;

public record TextEditorRenderBatch(
    TextEditorModel? Model,
    TextEditorViewModel? ViewModel,
    TextEditorOptions? Options);
