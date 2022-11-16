using BlazorTextEditor.RazorLib.Analysis.Html.InjectLanguage;

namespace BlazorTextEditor.RazorLib.Analysis.Razor;

public static class RazorInjectedLanguageFacts
{
    public static readonly InjectedLanguageDefinition
        RazorInjectedLanguageDefinition = new(
            "@",
            "@@",
            ParserInjectedLanguageFragmentCSharp
                .ParseInjectedLanguageFragment,
            new[]
            {
                new InjectedLanguageCodeBlock(
                    "@",
                    "{",
                    "}"),
                new InjectedLanguageCodeBlock(
                    "@",
                    "(",
                    ")"),
                new InjectedLanguageCodeBlock(
                    "@",
                    // @if (myExpression) { <div>true</div> }
                    "TODO: any control keyword (like for, if, or switch)",
                    "}"),
                new InjectedLanguageCodeBlock(
                    "@",
                    "code{",
                    "}"),
            });
}