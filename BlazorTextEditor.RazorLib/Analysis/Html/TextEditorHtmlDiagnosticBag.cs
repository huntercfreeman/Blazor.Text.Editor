using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html;

public class TextEditorHtmlDiagnosticBag : TextEditorDiagnosticBag
{
    public void ReportTagNameMissing(TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "Missing tag name.",
            textEditorTextSpan);
    }

    public void ReportOpenTagWithUnMatchedCloseTag(
        string openTagName,
        string closeTagName,
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Open tag: '{openTagName}' has an unmatched close tag: {closeTagName}.",
            textEditorTextSpan);
    }
}