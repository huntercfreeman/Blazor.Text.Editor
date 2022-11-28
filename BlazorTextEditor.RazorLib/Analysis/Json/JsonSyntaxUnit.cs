using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

namespace BlazorTextEditor.RazorLib.Analysis.Json;

public class JsonSyntaxUnit
{
    public JsonSyntaxUnit(
        JsonDocumentSyntax cssDocumentSyntax,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        JsonDocumentSyntax = cssDocumentSyntax;
        TextEditorJsonDiagnosticBag = textEditorJsonDiagnosticBag;
    }
    
    public JsonDocumentSyntax JsonDocumentSyntax { get; }
    public TextEditorJsonDiagnosticBag TextEditorJsonDiagnosticBag { get; }
}