using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;

public class InjectedLanguageDefinition
{
    public InjectedLanguageDefinition(
        string transitionSubstring, 
        string transitionSubstringEscaped,
        Func<StringWalker, TextEditorHtmlDiagnosticBag, InjectedLanguageDefinition, List<TagSyntax>> parseInjectedLanguageFunc)
    {
        TransitionSubstring = transitionSubstring;
        TransitionSubstringEscaped = transitionSubstringEscaped;
        ParseInjectedLanguageFunc = parseInjectedLanguageFunc;
    }

    /// <summary>
    /// Upon finding this substring when peeking by <see cref="TransitionSubstring"/>.Length
    /// the injected language Lexer will be invoked.
    /// </summary>
    public string TransitionSubstring { get; set; }
    /// <summary>
    /// If <see cref="TransitionSubstring"/> is found then a peek is done to ensure the upcoming
    /// text is not equal to <see cref="TransitionSubstringEscaped"/>.
    /// <br/><br/>
    /// Should both <see cref="TransitionSubstring"/> and <see cref="TransitionSubstringEscaped"/>
    /// be found, then the injected language Lexer will NOT be invoked.
    /// </summary>
    public string TransitionSubstringEscaped { get; set; }
    
    public Func<StringWalker, TextEditorHtmlDiagnosticBag, InjectedLanguageDefinition, List<TagSyntax>>
        ParseInjectedLanguageFunc { get; }
}