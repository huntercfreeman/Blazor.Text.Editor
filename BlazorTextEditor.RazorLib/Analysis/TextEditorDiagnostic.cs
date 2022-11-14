using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis;

public record TextEditorDiagnostic(
    DiagnosticLevel DiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);