using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

[FeatureState]
public class TextEditorGroupsCollection
{
    public TextEditorGroupsCollection()
    {
    }

    public ImmutableList<TextEditorGroup> GroupsList { get; init; } =
        ImmutableList<TextEditorGroup>.Empty;
}