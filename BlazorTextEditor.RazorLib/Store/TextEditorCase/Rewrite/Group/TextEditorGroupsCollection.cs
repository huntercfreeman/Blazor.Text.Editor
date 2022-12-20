using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

[FeatureState]
public class TextEditorGroupsCollection
{
    public TextEditorGroupsCollection()
    {
        GroupsMap = ImmutableDictionary<TextEditorGroupKey, TextEditorGroup>.Empty;
    }

    public ImmutableDictionary<TextEditorGroupKey, TextEditorGroup> GroupsMap { get; init; }
}