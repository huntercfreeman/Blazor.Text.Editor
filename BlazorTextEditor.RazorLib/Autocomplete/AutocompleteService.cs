namespace BlazorTextEditor.RazorLib.Autocomplete;

public class AutocompleteService : IAutocompleteService
{
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