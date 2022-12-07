using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

public interface IFSharpSyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IFSharpSyntax> Children { get; }
    public FSharpSyntaxKind FSharpSyntaxKind { get; }
}