using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Svelte.SyntaxActors;

namespace BlazorTextEditor.RazorLib.Analysis.Svelte.Facts;

public static class SvelteInjectedLanguageFacts
{
    public static readonly InjectedLanguageDefinition
        SvelteInjectedLanguageDefinition = new(
            "@",
            "@@",
            ParserInjectedLanguageFragmentSvelte
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