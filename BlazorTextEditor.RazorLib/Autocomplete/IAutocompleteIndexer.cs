using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Autocomplete;

public interface IAutocompleteIndexer : IDisposable
{
    public Task IndexAsync(TextEditorBase textEditorBase);
}