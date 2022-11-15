using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Autocomplete;

public class AutocompleteIndexer : IAutocompleteIndexer
{
    private readonly ITextEditorService _textEditorService;

    public AutocompleteIndexer(ITextEditorService textEditorService)
    {
        _textEditorService = textEditorService;
        
        _textEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;
    }

    public Task IndexAsync(TextEditorBase textEditorBase)
    {
        return Task.CompletedTask;
    }

    private void TextEditorServiceOnOnTextEditorStatesChanged()
    {
        // TODO: When should the indexer re-index or incrementally do so
    }
    
    public void Dispose()
    {
        _textEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    }
}