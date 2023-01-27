using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Diff;

public record TextEditorDiff(
    TextEditorDiffKey TextEditorDiffKey,
    TextEditorViewModelKey BeforeViewModelKey,
    TextEditorViewModelKey AfterViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}