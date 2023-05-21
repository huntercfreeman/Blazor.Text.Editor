using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis;

public record TextEditorDiagnostic(
    TextEditorDiagnosticLevel DiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);