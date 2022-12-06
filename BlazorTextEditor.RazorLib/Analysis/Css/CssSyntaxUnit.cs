using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public class CssSyntaxUnit
{
    public CssSyntaxUnit(
        CssDocumentSyntax cssDocumentSyntax,
        TextEditorCssDiagnosticBag textEditorCssDiagnosticBag)
    {
        CssDocumentSyntax = cssDocumentSyntax;
        TextEditorCssDiagnosticBag = textEditorCssDiagnosticBag;
    }
    
    public CssDocumentSyntax CssDocumentSyntax { get; }
    public TextEditorCssDiagnosticBag TextEditorCssDiagnosticBag { get; }
}