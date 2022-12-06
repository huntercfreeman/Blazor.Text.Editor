using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;

namespace BlazorTextEditor.RazorLib.Analysis.Html.ExtensionMethods;

public static class StringWalkerExtensions
{
    public static bool CheckForInjectedLanguageCodeBlockTag(
        this StringWalker stringWalker,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        return stringWalker.CheckForSubstring(injectedLanguageDefinition.InjectedLanguageCodeBlockTag) &&
               !stringWalker.CheckForSubstring(injectedLanguageDefinition.InjectedLanguageCodeBlockTagEscaped);
    }
}