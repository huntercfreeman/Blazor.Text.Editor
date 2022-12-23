using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

[FeatureState]
public class TextEditorViewModelsCollection
{
    public TextEditorViewModelsCollection()
    {
    }

    public ImmutableList<TextEditorViewModel> ViewModelsList { get; init; } = ImmutableList<TextEditorViewModel>.Empty;
}