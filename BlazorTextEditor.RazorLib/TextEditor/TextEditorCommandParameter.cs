using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.TextEditor;

public class TextEditorCommandParameter : ITextEditorCommandParameter
{
    public TextEditorCommandParameterKind TextEditorCommandParameterKind => 
        TextEditorCommandParameterKind.Default;
    
    public TextEditorBase TextEditor { get; }
    public ImmutableArray<TextEditorCursor> Cursors { get; }
}