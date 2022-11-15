﻿using BlazorTextEditor.RazorLib.Analysis.CSharp;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Razor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.DialogCase;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IDispatcher _dispatcher;
    private readonly IStorageProvider _storageProvider;
    private readonly IState<TextEditorStates> _textEditorStates;
    

    public TextEditorService(
        IState<TextEditorStates> textEditorStates,
        IDispatcher dispatcher,
        IStorageProvider storageProvider)
    {
        _textEditorStates = textEditorStates;
        _dispatcher = dispatcher;
        _storageProvider = storageProvider;

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

    public void RegisterCustomTextEditor(
        TextEditorBase textEditorBase)
    {
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterCSharpTextEditor(
        TextEditorKey textEditorKey, 
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            initialContent,
            new TextEditorCSharpLexer(),
            new TextEditorCSharpDecorationMapper(),
            null,
            textEditorKey);
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterRazorTextEditor(TextEditorKey textEditorKey, string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            initialContent,
            new TextEditorRazorLexer(),
            new TextEditorHtmlDecorationMapper(),
            null,
            textEditorKey);
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterHtmlTextEditor(TextEditorKey textEditorKey, string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            initialContent,
            new TextEditorHtmlLexer(),
            new TextEditorHtmlDecorationMapper(),
            null,
            textEditorKey);
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterPlainTextEditor(TextEditorKey textEditorKey, string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            initialContent,
            null,
            null,
            null,
            textEditorKey);
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void InsertText(InsertTextTextEditorBaseAction insertTextTextEditorBaseAction)
    {
        _dispatcher.Dispatch(insertTextTextEditorBaseAction);
    }

    public void HandleKeyboardEvent(KeyboardEventTextEditorBaseAction keyboardEventTextEditorBaseAction)
    {
        _dispatcher.Dispatch(keyboardEventTextEditorBaseAction);
    }
    
    public void DeleteTextByMotion(DeleteTextByMotionTextEditorBaseAction deleteTextByMotionTextEditorBaseAction)
    {
        _dispatcher.Dispatch(deleteTextByMotionTextEditorBaseAction);
    }
    
    public void DeleteTextByRange(DeleteTextByRangeTextEditorBaseAction deleteTextByRangeTextEditorBaseAction)
    {
        _dispatcher.Dispatch(deleteTextByRangeTextEditorBaseAction);
    }
    
    public void RedoEdit(TextEditorKey textEditorKey)
    {
        var redoEditAction = new RedoEditAction(textEditorKey);
        
        _dispatcher.Dispatch(redoEditAction);
    }
    
    public void UndoEdit(TextEditorKey textEditorKey)
    {
        var undoEditAction = new UndoEditAction(textEditorKey);
        
        _dispatcher.Dispatch(undoEditAction);
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

    public void ShowSettingsDialog()
    {
        var settingsDialog = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Text Editor Settings",
            typeof(TextEditorSettings),
            null);
        
        _dispatcher.Dispatch(
            new RegisterDialogRecordAction(
                settingsDialog));
    }
    
    
    /// <summary>
    /// <see cref="ForceRerender"/> 
    /// </summary>
    public void ForceRerender(TextEditorKey textEditorKey)
    {
        _dispatcher.Dispatch(
            new ForceRerenderAction(
                textEditorKey));
    }

    private void TextEditorStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnTextEditorStatesChanged?.Invoke();
    }
    
    public async Task SetTextEditorOptionsFromLocalStorageAsync()
    {
        var optionsJsonString = (await _storageProvider
            .GetValue(ITextEditorService.LocalStorageGlobalTextEditorOptionsKey))
                as string;

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