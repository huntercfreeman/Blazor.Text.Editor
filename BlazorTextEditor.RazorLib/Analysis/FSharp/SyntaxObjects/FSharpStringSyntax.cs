using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxObjects;

public class FSharpStringSyntax : IFSharpSyntax
{
    public FSharpStringSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IFSharpSyntax> Children => ImmutableArray<IFSharpSyntax>.Empty;
    public FSharpSyntaxKind FSharpSyntaxKind => FSharpSyntaxKind.String;
}