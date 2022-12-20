using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Model;

[FeatureState]
public class TextEditorModelsCollection
{
    public TextEditorModelsCollection()
    {
        ModelsMap = ImmutableDictionary<TextEditorModelKey, TextEditorModel>.Empty;
    }

    public ImmutableDictionary<TextEditorModelKey, TextEditorModel> ModelsMap { get; init; }
}