using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.Facts;

public static class RazorInjectedLanguageFacts
{
    public static readonly InjectedLanguageDefinition
        RazorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            RazorSyntaxTree
                .ParseInjectedLanguageFragment);
}