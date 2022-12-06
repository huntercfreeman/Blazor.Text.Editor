using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;

public class InjectedLanguageDefinition
{
    public InjectedLanguageDefinition(
        string injectedLanguageCodeBlockTag,
        string injectedLanguageCodeBlockTagEscaped,
        Func<StringWalker, TextEditorHtmlDiagnosticBag, InjectedLanguageDefinition, List<TagSyntax>> parseInjectedLanguageFunc, 
        InjectedLanguageCodeBlock[] injectedLanguageCodeBlocks)
    {
        InjectedLanguageCodeBlockTag = injectedLanguageCodeBlockTag;
        InjectedLanguageCodeBlockTagEscaped = injectedLanguageCodeBlockTagEscaped;
        ParseInjectedLanguageFunc = parseInjectedLanguageFunc;
        InjectedLanguageCodeBlocks = injectedLanguageCodeBlocks;
    }

    public string InjectedLanguageCodeBlockTag { get; }
    public string InjectedLanguageCodeBlockTagEscaped { get; }
    public Func<StringWalker, TextEditorHtmlDiagnosticBag, InjectedLanguageDefinition, List<TagSyntax>>
        ParseInjectedLanguageFunc { get; }
    public InjectedLanguageCodeBlock[] InjectedLanguageCodeBlocks { get; }
}