namespace BlazorTextEditor.ClassLib.TextEditor;

public record EditBlock(
    TextEditKind TextEditKind, 
    string DisplayName, 
    string ContentSnapshot);