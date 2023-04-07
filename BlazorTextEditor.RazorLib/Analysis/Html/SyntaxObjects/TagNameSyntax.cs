using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

public class TagNameSyntax : IHtmlSyntax
{
    public TagNameSyntax(
        string value,
        TextEditorTextSpan textEditorTextSpan)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    /// <summary>Likely storing the value here is incorrect. It is believed one should never store values of lexed tokens but instead do a lookup into the original text using the text editor text span.</summary>
    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TagName;
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IHtmlSyntax>.Empty;
}