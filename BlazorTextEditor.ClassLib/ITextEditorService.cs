using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;

namespace BlazorTextEditor.ClassLib;

public interface ITextEditorService : IDisposable
{
    public TextEditorStates TextEditorStates { get; }
    
    public event EventHandler? OnTextEditorStatesChanged;

    public void RegisterTextEditor(TextEditorBase textEditorBase);
    public void EditTextEditor(EditTextEditorBaseAction editTextEditorBaseAction);
    public void DisposeTextEditor(TextEditorKey textEditorKey);
}