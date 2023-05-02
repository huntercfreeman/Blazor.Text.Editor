using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Group;

/// <summary>Store the state of none or many tabs, and which tab is the active one. Each tab represents a <see cref="TextEditorViewModel"/>.</summary>
public record TextEditorGroup(
    TextEditorGroupKey GroupKey,
    TextEditorViewModelKey ActiveViewModelKey,
    ImmutableList<TextEditorViewModelKey> ViewModelKeys)
{
    public TextEditorStateChangedKey TextEditorStateChangedKey { get; init; } = 
        TextEditorStateChangedKey.NewTextEditorStateChangedKey();
}