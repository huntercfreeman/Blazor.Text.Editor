using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.View;

[FeatureState]
public class TextEditorViewsCollection
{
    public TextEditorViewsCollection()
    {
        ViewsMap = ImmutableDictionary<TextEditorViewKey, TextEditorView>.Empty;
    }

    public ImmutableDictionary<TextEditorViewKey, TextEditorView> ViewsMap { get; init; }
}