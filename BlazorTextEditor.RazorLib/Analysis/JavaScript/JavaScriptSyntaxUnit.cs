namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxUnit
{
    public JavaScriptSyntaxUnit(
        JavaScriptDocumentSyntax javaScriptDocumentSyntax, 
        TextEditorDiagnosticBag textEditorDiagnosticBag)
    {
        JavaScriptDocumentSyntax = javaScriptDocumentSyntax;
        TextEditorDiagnosticBag = textEditorDiagnosticBag;
    }
    
    public JavaScriptDocumentSyntax JavaScriptDocumentSyntax { get; }
    public TextEditorDiagnosticBag TextEditorDiagnosticBag { get; }
}