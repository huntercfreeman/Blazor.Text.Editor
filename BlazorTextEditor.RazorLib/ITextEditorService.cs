using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib;

public interface ITextEditorService : IDisposable
{
    public TextEditorStates TextEditorStates { get; }
    
    public event EventHandler? OnTextEditorStatesChanged;

    public void RegisterTextEditor(TextEditorBase textEditorBase);
    public void EditTextEditor(EditTextEditorBaseAction editTextEditorBaseAction);
    public void DisposeTextEditor(TextEditorKey textEditorKey);
    public void SetFontSize(int fontSizeInPixels);
}