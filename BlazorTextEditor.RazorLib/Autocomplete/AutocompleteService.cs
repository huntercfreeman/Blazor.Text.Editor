namespace BlazorTextEditor.RazorLib.Autocomplete;

public class AutocompleteService : IAutocompleteService
{
    private readonly IAutocompleteIndexer _autocompleteIndexer;

    public AutocompleteService(IAutocompleteIndexer autocompleteIndexer)
    {
        _autocompleteIndexer = autocompleteIndexer;
    }
    
    public List<string> GetAutocompleteOptions(string word)
    {
        return new List<string>
        {
            "TestA",
            "TestB",
            "TestC",
        };
    }
}