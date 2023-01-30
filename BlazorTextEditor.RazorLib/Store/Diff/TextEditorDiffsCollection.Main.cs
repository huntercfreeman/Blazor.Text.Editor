using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Diff;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Diff;

/// <summary>
/// Keep the <see cref="TextEditorDiffsCollection"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorDiffsCollection
{
    public TextEditorDiffsCollection()
    {
        DiffsList = ImmutableList<TextEditorDiff>.Empty; 
    }

    public ImmutableList<TextEditorDiff> DiffsList { get; init; }
}