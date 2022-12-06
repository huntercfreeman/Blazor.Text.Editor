using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

public class FSharpSyntaxUnit
{
    public FSharpSyntaxUnit(
        FSharpDocumentSyntax fSharpDocumentSyntax, 
        TextEditorDiagnosticBag textEditorDiagnosticBag)
    {
        FSharpDocumentSyntax = fSharpDocumentSyntax;
        TextEditorDiagnosticBag = textEditorDiagnosticBag;
    }
    
    public FSharpDocumentSyntax FSharpDocumentSyntax { get; }
    public TextEditorDiagnosticBag TextEditorDiagnosticBag { get; }
}