using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.Facts;

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