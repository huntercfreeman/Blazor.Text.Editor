using BlazorTextEditor.RazorLib.Find;
using Fluxor;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Store.Find;

/// <summary>
/// Keep the <see cref="TextEditorFindProvidersCollection"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorFindProvidersCollection
{
    private TextEditorFindProvidersCollection()
    {
        FindProvidersList = ImmutableList<ITextEditorFindProvider>.Empty;
        ActiveTextEditorFindProviderKey = TextEditorFindProviderKey.Empty;
    }

    public TextEditorFindProvidersCollection(
        ImmutableList<ITextEditorFindProvider> findProvidersList,
        TextEditorFindProviderKey activeTextEditorFindProviderKey)
    {
        FindProvidersList = findProvidersList;
        ActiveTextEditorFindProviderKey = activeTextEditorFindProviderKey;
    }

    public ImmutableList<ITextEditorFindProvider> FindProvidersList { get; init; }
    public TextEditorFindProviderKey ActiveTextEditorFindProviderKey { get; init; }

    public ITextEditorFindProvider? ActiveFindProviderOrDefault()
    {
        return FindProvidersList.FirstOrDefault(x =>
            x.FindProviderKey == ActiveTextEditorFindProviderKey);
    }
}
