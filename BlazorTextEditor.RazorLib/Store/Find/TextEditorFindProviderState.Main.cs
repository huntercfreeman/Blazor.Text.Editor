using BlazorTextEditor.RazorLib.Find;
using Fluxor;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Store.Find;

/// <summary>
/// Keep the <see cref="TextEditorFindProviderState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorFindProviderState
{
    private TextEditorFindProviderState()
    {
        FindProvidersList = ImmutableList<ITextEditorFindProvider>.Empty;
        ActiveFindProviderKey = TextEditorFindProviderKey.Empty;
        SearchQuery = string.Empty;
    }

    public TextEditorFindProviderState(
        ImmutableList<ITextEditorFindProvider> findProvidersList,
        TextEditorFindProviderKey activeTextEditorFindProviderKey,
        string searchQuery)
    {
        FindProvidersList = findProvidersList;
        ActiveFindProviderKey = activeTextEditorFindProviderKey;
        SearchQuery = searchQuery;
    }

    public ImmutableList<ITextEditorFindProvider> FindProvidersList { get; init; }
    public TextEditorFindProviderKey ActiveFindProviderKey { get; init; }
    public string SearchQuery { get; init; }

    public ITextEditorFindProvider? ActiveFindProviderOrDefault()
    {
        return FindProvidersList.FirstOrDefault(x =>
            x.FindProviderKey == ActiveFindProviderKey);
    }
}
