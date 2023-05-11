using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Find;

public static class FindFacts
{
    public static readonly ImmutableArray<ITextEditorFindProvider> DefaultFindProviders = new ITextEditorFindProvider[]
    {
        new RegisteredViewModelsFindProvider(),
        new RenderedViewModelsFindProvider(),
    }.ToImmutableArray();
}
