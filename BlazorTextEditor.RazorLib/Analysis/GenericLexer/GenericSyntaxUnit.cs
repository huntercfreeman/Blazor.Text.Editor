using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer;

public class GenericSyntaxUnit
{
    public GenericSyntaxUnit(
        GenericDocumentSyntax genericDocumentSyntax,
        TextEditorDiagnosticBag textEditorDiagnosticBag)
    {
        GenericDocumentSyntax = genericDocumentSyntax;
        TextEditorDiagnosticBag = textEditorDiagnosticBag;
    }
    
    public GenericDocumentSyntax GenericDocumentSyntax { get; }
    public TextEditorDiagnosticBag TextEditorDiagnosticBag { get; }
}