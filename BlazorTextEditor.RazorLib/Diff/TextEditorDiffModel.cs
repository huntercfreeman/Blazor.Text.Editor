using BlazorTextEditor.RazorLib.Store.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Diff;

public record TextEditorDiffModel(
    TextEditorDiffKey DiffKey,
    TextEditorViewModelKey BeforeViewModelKey,
    TextEditorViewModelKey AfterViewModelKey)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}