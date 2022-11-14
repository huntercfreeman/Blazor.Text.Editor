namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

public enum TagKind
{
    Opening,
    Closing,
    SelfClosing,
    Text,
    InjectedLanguageCodeBlock,
}