namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;

public enum TagKind
{
    Opening,
    Closing,
    SelfClosing,
    Text,
    InjectedLanguageCodeBlock,
}