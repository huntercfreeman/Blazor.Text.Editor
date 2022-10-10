using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.ClassLib;

public class TextEditorService : ITextEditorService, IDisposable
{
    private readonly IState<TextEditorStates> _textEditorStates;
    private readonly IDispatcher _dispatcher;

    public TextEditorService(
        IState<TextEditorStates> textEditorStates, 
        IDispatcher dispatcher)
    {
        _textEditorStates = textEditorStates;
        _dispatcher = dispatcher;

        _textEditorStates.StateChanged += TextEditorStatesOnStateChanged;
    }

    public TextEditorStates TextEditorStates => _textEditorStates.Value;
    
    public event EventHandler? OnTextEditorStatesChanged;

    public void RegisterTextEditor(TextEditorBase textEditorBase)
    {
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }
    
    public void EditTextEditor(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        _dispatcher.Dispatch(editTextEditorBaseAction);
    }
    
    public void DisposeTextEditor(TextEditorKey textEditorKey)
    {
        _dispatcher.Dispatch(
            new DisposeTextEditorBaseAction(textEditorKey));
    }
    
    private void TextEditorStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnTextEditorStatesChanged?.Invoke(sender, e);
    }

    public void Dispose()
    {
        _textEditorStates.StateChanged -= TextEditorStatesOnStateChanged;
    }
}