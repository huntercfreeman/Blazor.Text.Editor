using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxObjects;

public class FSharpDocumentSyntax : IFSharpSyntax
{
    public FSharpDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan, 
        ImmutableArray<IFSharpSyntax> children)
    {
        TextEditorTextSpan = textEditorTextSpan;
        Children = children;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IFSharpSyntax> Children { get; }
    public FSharpSyntaxKind FSharpSyntaxKind => FSharpSyntaxKind.Document;
}