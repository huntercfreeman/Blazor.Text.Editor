using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Group;

public record TextEditorGroup(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey ActiveTextEditorViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}