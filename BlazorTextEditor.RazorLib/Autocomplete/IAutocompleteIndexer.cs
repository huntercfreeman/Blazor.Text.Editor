using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Autocomplete;

public interface IAutocompleteIndexer : IDisposable
{
    public ImmutableArray<string> IndexedStrings { get; }
    
    public Task IndexTextEditorAsync(TextEditorBase textEditorBase);
    public Task IndexWordAsync(string word);
}