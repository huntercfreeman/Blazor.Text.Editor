namespace FictitiousLanguage.ClassLib.Classes;

public static class SyntaxTokenFacts
{
    public static class Keywords
    {
        public const string VARIABLE_DECLARATION_KEYWORD = "var";
    }
    
    public static readonly List<string> KeywordsList = new()
    {
        Keywords.VARIABLE_DECLARATION_KEYWORD
    };
}