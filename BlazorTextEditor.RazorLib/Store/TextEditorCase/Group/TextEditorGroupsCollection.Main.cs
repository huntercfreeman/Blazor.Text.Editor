using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Group;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

[FeatureState]
public partial class TextEditorGroupsCollection
{
    public TextEditorGroupsCollection()
    {
    }

    /// <summary>
    /// Keep the <see cref="TextEditorGroupsCollection"/> as a class
    /// as to avoid record value comparisons when Fluxor checks
    /// if the <see cref="FeatureStateAttribute"/> has been replaced.
    /// </summary>
    public ImmutableList<TextEditorGroup> GroupsList { get; init; } =
        ImmutableList<TextEditorGroup>.Empty;
}