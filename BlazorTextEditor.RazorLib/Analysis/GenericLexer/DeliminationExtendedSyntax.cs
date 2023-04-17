using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer;

/// <summary>
/// An example of usage would be for the C language's preprocessor.
/// Given "#include &lt;stdlib.h&gt;" one would syntax highlight "&lt;stdlib.h&gt;"
/// as if it were a string.
/// </summary>
public class DeliminationExtendedSyntax
{
    public DeliminationExtendedSyntax(
        string syntaxStart,
        string syntaxEnd,
        Func<TextEditorTextSpan, IGenericSyntax> genericSyntaxFactory,
        GenericDecorationKind genericDecorationKind)
    {
        SyntaxStart = syntaxStart;
        SyntaxEnd = syntaxEnd;
        GenericSyntaxFactory = genericSyntaxFactory;
        GenericDecorationKind = genericDecorationKind;
    }
    
    public string SyntaxStart { get; }
    public string SyntaxEnd { get; }
    public Func<TextEditorTextSpan, IGenericSyntax> GenericSyntaxFactory { get; }
    public GenericDecorationKind GenericDecorationKind { get; }
}