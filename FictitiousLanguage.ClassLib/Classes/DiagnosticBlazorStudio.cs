namespace FictitiousLanguage.ClassLib.Classes;

public class DiagnosticBlazorStudio
{
    public DiagnosticBlazorStudio(string message, DiagnosticLevel diagnosticLevel)
    {
        Message = message;
        DiagnosticLevel = diagnosticLevel;
    }

    public string Message { get; }
    public DiagnosticLevel DiagnosticLevel { get; set; }
}