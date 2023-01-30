using BlazorTextEditor.RazorLib.Store.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Diff;

/// <summary>
/// TODO: Continue working on the diff editor
/// </summary>
public record TextEditorDiff(
    TextEditorDiffKey DiffKey,
    TextEditorViewModelKey BeforeViewModelKey,
    TextEditorViewModelKey AfterViewModelKey)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}