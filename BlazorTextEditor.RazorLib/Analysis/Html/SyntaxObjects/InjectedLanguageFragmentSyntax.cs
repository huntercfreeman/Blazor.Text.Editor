using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

public class InjectedLanguageFragmentSyntax : TagSyntax, IHtmlSyntax
{
    public InjectedLanguageFragmentSyntax(
        ImmutableArray<IHtmlSyntax> childHtmlSyntaxes,
        string value,
        TextEditorTextSpan textEditorTextSpan,
        bool hasSpecialHtmlCharacter = false)
        : base(
            null,
            null,
            ImmutableArray<AttributeSyntax>.Empty,
            childHtmlSyntaxes,
            TagKind.InjectedLanguageCodeBlock,
            hasSpecialHtmlCharacter)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public override HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.InjectedLanguageFragment;
}