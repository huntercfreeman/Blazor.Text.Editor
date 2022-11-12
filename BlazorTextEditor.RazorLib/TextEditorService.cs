using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IDispatcher _dispatcher;
    private readonly IJSRuntime _jsRuntime;
    private readonly IState<TextEditorStates> _textEditorStates;
    

    public TextEditorService(
        IState<TextEditorStates> textEditorStates,
        IDispatcher dispatcher,
        IJSRuntime jsRuntime)
    {
        _textEditorStates = textEditorStates;
        _dispatcher = dispatcher;
        _jsRuntime = jsRuntime;

        _textEditorStates.StateChanged += TextEditorStatesOnStateChanged;
    }

    public TextEditorStates TextEditorStates => _textEditorStates.Value;

    public string GlobalThemeCssClassString => TextEditorStates
                                                   .GlobalTextEditorOptions
                                                   .Theme?
                                                   .CssClassString
                                               ?? string.Empty;

    public Theme? GlobalThemeValue => TextEditorStates
        .GlobalTextEditorOptions
        .Theme;

    public string GlobalFontSizeInPixelsStyling => "font-size: " + 
                                                   GlobalFontSizeInPixelsValue +
                                                   "px;";
    
    public int GlobalFontSizeInPixelsValue => TextEditorStates
        .GlobalTextEditorOptions
        .FontSizeInPixels!.Value;

    public bool GlobalShowNewlines => TextEditorStates.GlobalTextEditorOptions.ShowNewlines!.Value;

    public bool GlobalShowWhitespace => TextEditorStates.GlobalTextEditorOptions.ShowWhitespace!.Value;

    public event Action? OnTextEditorStatesChanged;

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
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetTheme(Theme theme)
    {
        _dispatcher.Dispatch(
            new TextEditorSetThemeAction(theme));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetShowWhitespace(bool showWhitespace)
    {
        _dispatcher.Dispatch(
            new TextEditorSetShowWhitespaceAction(showWhitespace));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetShowNewlines(bool showNewlines)
    {
        _dispatcher.Dispatch(
            new TextEditorSetShowNewlinesAction(showNewlines));

        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetUsingRowEndingKind(TextEditorKey textEditorKey, RowEndingKind rowEndingKind)
    {
        _dispatcher.Dispatch(
            new TextEditorSetUsingRowEndingKindAction(textEditorKey, rowEndingKind));
    }

    private void TextEditorStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnTextEditorStatesChanged?.Invoke();
    }
    
    public async Task SetTextEditorOptionsFromLocalStorageAsync()
    {
        var optionsJsonString = await _jsRuntime.InvokeAsync<string>(
            "blazorTextEditor.localStorageGetItem",
            ITextEditorService.LocalStorageGlobalTextEditorOptionsKey);

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;
        
        var options = System.Text.Json.JsonSerializer
            .Deserialize<TextEditorOptions>(optionsJsonString);

        if (options is null)
            return;
        
        if (options.Theme is not null)
            SetTheme(options.Theme);
        
        if (options.FontSizeInPixels is not null)
            SetFontSize(options.FontSizeInPixels.Value);
        
        if (options.ShowNewlines is not null)
            SetShowNewlines(options.ShowNewlines.Value);
        
        if (options.ShowWhitespace is not null)
            SetShowWhitespace(options.ShowWhitespace.Value);
    }
    
    public void WriteGlobalTextEditorOptionsToLocalStorage()
    {
        _dispatcher.Dispatch(
            new StorageEffects.WriteGlobalTextEditorOptionsToLocalStorageAction(
                TextEditorStates.GlobalTextEditorOptions));
    }
    
    public void Dispose()
    {
        _textEditorStates.StateChanged -= TextEditorStatesOnStateChanged;
    }
}