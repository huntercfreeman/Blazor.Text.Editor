using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
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

    public void SetFontSize(int fontSizeInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorSetFontSizeAction(fontSizeInPixels));
    }
    
    public void SetTheme(Theme theme)
    {
        _dispatcher.Dispatch(
            new TextEditorSetThemeAction(theme));
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