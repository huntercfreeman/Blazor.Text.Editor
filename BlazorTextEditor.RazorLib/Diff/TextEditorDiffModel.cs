using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Diff;

public record TextEditorDiffModel(
    TextEditorDiffKey DiffKey,
    TextEditorViewModelKey BeforeViewModelKey,
    TextEditorViewModelKey AfterViewModelKey)
{
    public RenderStateKey RenderStateKey { get; init; } = RenderStateKey.NewRenderStateKey();
}