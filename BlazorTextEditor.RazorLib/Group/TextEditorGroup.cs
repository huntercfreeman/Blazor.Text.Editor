using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Group;

/// <summary>Store the state of none or many tabs, and which tab is the active one. Each tab represents a <see cref="TextEditorViewModel"/>.</summary>
public record TextEditorGroup(
    TextEditorGroupKey GroupKey,
    TextEditorViewModelKey ActiveViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}