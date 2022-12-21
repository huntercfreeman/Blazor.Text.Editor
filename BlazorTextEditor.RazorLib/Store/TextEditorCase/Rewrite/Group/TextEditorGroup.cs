using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

public record TextEditorGroup(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey ActiveTextEditorViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}