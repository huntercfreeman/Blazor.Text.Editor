using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class TextEditorCssLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(text);
        
        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var cssSyntaxWalker = new CssSyntaxWalker();

        cssSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssIdentifierSyntaxes.Select(identifier =>
                identifier.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssCommentSyntaxes.Select(comment =>
                comment.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyNameSyntaxes.Select(propertyName =>
                propertyName.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyValueSyntaxes.Select(propertyValue =>
                propertyValue.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}