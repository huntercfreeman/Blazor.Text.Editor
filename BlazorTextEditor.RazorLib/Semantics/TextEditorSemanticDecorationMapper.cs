using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Diff;

public class TextEditorSemanticDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TextEditorSemanticDecorationKind)decorationByte;

        return decoration switch
        {
            TextEditorSemanticDecorationKind.None => string.Empty,
            TextEditorSemanticDecorationKind.DiagnosticError => "bte_semantic-diagnostic-error",
            TextEditorSemanticDecorationKind.DiagnosticHint => "bte_semantic-diagnostic-hint",
            TextEditorSemanticDecorationKind.DiagnosticSuggestion => "bte_semantic-diagnostic-suggestion",
            TextEditorSemanticDecorationKind.DiagnosticWarning => "bte_semantic-diagnostic-warning",
            TextEditorSemanticDecorationKind.DiagnosticOther => "bte_semantic-diagnostic-other",
            _ => string.Empty,
        };
    }
}