using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public record TextEditorGroup(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey ActiveTextEditorViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}