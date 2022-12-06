using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;

namespace BlazorTextEditor.RazorLib.Analysis.Html.ExtensionMethods;

public static class StringWalkerExtensions
{
    public static bool CheckForInjectedLanguageCodeBlockTag(
        this StringWalker stringWalker,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var isMatch = stringWalker.CheckForSubstring(injectedLanguageDefinition.InjectedLanguageCodeBlockTag);
        var isEscaped = stringWalker.CheckForSubstring(injectedLanguageDefinition.InjectedLanguageCodeBlockTagEscaped);
        
        return isMatch && !isEscaped;
    }
}