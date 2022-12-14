using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.Shared.Facts;
using BlazorALaCarte.Shared.Storage;
using BlazorALaCarte.Shared.Store;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Css.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.Decoration;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Json.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.Decoration;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IState<TextEditorStates> _textEditorStates;
    private readonly IState<ThemeState> _themeState;
    private readonly IState<TextEditorViewModelsCollection> _textEditorViewModelsCollection;
    private readonly IDispatcher _dispatcher;
    private readonly IStorageProvider _storageProvider;
    
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;

    public TextEditorService(
        IState<TextEditorStates> textEditorStates,
        IState<ThemeState> themeState,
        IState<TextEditorViewModelsCollection> textEditorViewModelsCollection,
        IDispatcher dispatcher,
        IStorageProvider storageProvider,
        IJSRuntime jsRuntime)
    {
        _textEditorStates = textEditorStates;
        _themeState = themeState;
        _textEditorViewModelsCollection = textEditorViewModelsCollection;
        _dispatcher = dispatcher;
        _storageProvider = storageProvider;
        _jsRuntime = jsRuntime;

        _textEditorStates.StateChanged += TextEditorStatesOnStateChanged;
    }

    public TextEditorStates TextEditorStates => _textEditorStates.Value;
    public ThemeRecord? GlobalThemeValue => TextEditorStates.GlobalTextEditorOptions.Theme;
    public string GlobalThemeCssClassString => TextEditorStates.GlobalTextEditorOptions.Theme?.CssClassString ?? string.Empty;
    public string GlobalFontSizeInPixelsStyling => $"font-size: {TextEditorStates.GlobalTextEditorOptions.FontSizeInPixels!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";
    public bool GlobalShowNewlines => TextEditorStates.GlobalTextEditorOptions.ShowNewlines!.Value;
    public bool GlobalShowWhitespace => TextEditorStates.GlobalTextEditorOptions.ShowWhitespace!.Value;
    public int GlobalFontSizeInPixelsValue => TextEditorStates.GlobalTextEditorOptions.FontSizeInPixels!.Value;
    public double GlobalCursorWidthInPixelsValue => TextEditorStates.GlobalTextEditorOptions.CursorWidthInPixels!.Value;
    public KeymapDefinition GlobalKeymapDefinition => TextEditorStates.GlobalTextEditorOptions.KeymapDefinition!;
    public int? GlobalHeightInPixelsValue => TextEditorStates.GlobalTextEditorOptions.HeightInPixels;

    public event Action? OnTextEditorStatesChanged;
    
    public void RegisterCustomTextEditor(
        TextEditorBase textEditorBase)
    {
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterCSharpTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorCSharpLexer(),
            new TextEditorCSharpDecorationMapper(),
            null,
            textEditorKey);

        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }
    
    public void RegisterHtmlTextEditor(
        TextEditorKey textEditorKey, 
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorHtmlLexer(),
            new TextEditorHtmlDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterCssTextEditor(
        TextEditorKey textEditorKey, 
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorCssLexer(),
            new TextEditorCssDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }
    
    public void RegisterJsonTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorJsonLexer(),
            new TextEditorJsonDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterFSharpTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorFSharpLexer(),
            new TextEditorFSharpDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterRazorTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorRazorLexer(),
            new TextEditorHtmlDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }

    public void RegisterJavaScriptTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorJavaScriptLexer(),
            new TextEditorJavaScriptDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }
    
    public void RegisterTypeScriptTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new TextEditorTypeScriptLexer(),
            new TextEditorTypeScriptDecorationMapper(),
            null,
            textEditorKey);
        
        _ = Task.Run(async () =>
        {
            await textEditorBase.ApplySyntaxHighlightingAsync();
        });
        
        _dispatcher.Dispatch(
            new RegisterTextEditorBaseAction(textEditorBase));
    }
    
    public void RegisterPlainTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null)
    {
        var textEditorBase = new TextEditorBase(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
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
    
    public void SetCursorWidth(double cursorWidthInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorSetCursorWidthAction(cursorWidthInPixels));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }
    
    public void SetHeight(int? heightInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorSetHeightAction(heightInPixels));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetTheme(ThemeRecord theme)
    {
        _dispatcher.Dispatch(
            new TextEditorSetThemeAction(theme));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }
    
    public void SetKeymap(KeymapDefinition foundKeymap)
    {
        _dispatcher.Dispatch(
            new TextEditorSetKeymapAction(foundKeymap));
        
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
    
    public void SetResourceData(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime)
    {
        _dispatcher.Dispatch(
            new TextEditorSetResourceDataAction(
                textEditorKey,
                resourceUri,
                resourceLastWriteTime));
    }

    public void ShowSettingsDialog(bool isResizable = false)
    {
        var settingsDialog = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Text Editor Settings",
            typeof(TextEditorSettings),
            null)
        {
            IsResizable = isResizable
        };
        
        _dispatcher.Dispatch(
            new DialogsState.RegisterDialogRecordAction(
                settingsDialog));
    }
    
    private void TextEditorStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnTextEditorStatesChanged?.Invoke();
    }

    public void RegisterGroup(TextEditorGroupKey textEditorGroupKey)
    {
        var textEditorGroup = new TextEditorGroup(
            textEditorGroupKey,
            TextEditorViewModelKey.Empty,
            ImmutableList<TextEditorViewModelKey>.Empty);
        
        _dispatcher.Dispatch(new RegisterTextEditorGroupAction(textEditorGroup));
    }

    public void AddViewModelToGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new AddViewModelToGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
    }
    
    public void RemoveViewModelFromGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new RemoveViewModelFromGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
    }
    
    public void SetActiveViewModelOfGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(new SetActiveViewModelOfGroupAction(
            textEditorGroupKey,
            textEditorViewModelKey));
    }

    public void RegisterViewModel(
        TextEditorViewModelKey textEditorViewModelKey,
        TextEditorKey textEditorKey)
    {
        _dispatcher.Dispatch(new RegisterTextEditorViewModelAction(
            textEditorViewModelKey,
            textEditorKey, 
            this));
    }

    public ImmutableArray<TextEditorViewModel> GetViewModelsForTextEditorBase(TextEditorKey textEditorKey)
    {
        return _textEditorViewModelsCollection.Value.ViewModelsList
            .Where(x => x.TextEditorKey == textEditorKey)
            .ToImmutableArray();
    }

    public TextEditorBase? GetTextEditorBaseFromViewModelKey(TextEditorViewModelKey textEditorViewModelKey)
    {
        var textEditorViewModelsCollection = _textEditorViewModelsCollection.Value;
        
        var viewModel = textEditorViewModelsCollection.ViewModelsList
            .FirstOrDefault(x => 
                x.TextEditorViewModelKey == textEditorViewModelKey);
        
        if (viewModel is null)
            return null;
        
        var localTextEditorStates = TextEditorStates;

        return localTextEditorStates.TextEditorList
            .FirstOrDefault(x => x.Key == viewModel.TextEditorKey);
    }

    public void SetViewModelWith(
        TextEditorViewModelKey textEditorViewModelKey,
        Func<TextEditorViewModel, TextEditorViewModel> withFunc)
    {
        _dispatcher.Dispatch(new SetViewModelWithAction(
            textEditorViewModelKey,
            withFunc));
    }

    public async Task SetGutterScrollTopAsync(string gutterElementId, double scrollTop)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.setGutterScrollTop",
            gutterElementId,
            scrollTop);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }

    public async Task MutateScrollHorizontalPositionByPixelsAsync(
        string bodyElementId,
        string gutterElementId,
        double pixels)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollHorizontalPositionByPixels",
            bodyElementId,
            gutterElementId,
            pixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(
        string bodyElementId,
        string gutterElementId,
        double pixels)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollVerticalPositionByPixels",
            bodyElementId,
            gutterElementId,
            pixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }

    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public async Task SetScrollPositionAsync(
        string bodyElementId,
        string gutterElementId,
        double? scrollLeft,
        double? scrollTop)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.setScrollPosition",
            bodyElementId,
            gutterElementId,
            scrollLeft,
            scrollTop);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }

    public async Task<ElementMeasurementsInPixels> GetElementMeasurementsInPixelsById(string elementId)
    {
        return await _jsRuntime.InvokeAsync<ElementMeasurementsInPixels>(
            "blazorTextEditor.getElementMeasurementsInPixelsById",
            elementId);
    }
    
    public TextEditorBase? GetTextEditorBaseOrDefaultByResourceUri(string resourceUri)
    {
        return TextEditorStates.TextEditorList
            .FirstOrDefault(x => x.ResourceUri == resourceUri);
    }
    
    public void ReloadTextEditorBase(
        TextEditorKey textEditorKey,
        string content)
    {
        _dispatcher.Dispatch(
            new ReloadTextEditorBaseAction(
                textEditorKey,
                content));
    }
    
    public async Task FocusPrimaryCursorAsync(string primaryCursorContentId)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.focusHtmlElementById",
            primaryCursorContentId);
    }
    
    public async Task SetTextEditorOptionsFromLocalStorageAsync()
    {
        var optionsJsonString = (await _storageProvider
            .GetValue(ITextEditorService.LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY))
                as string;

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;
        
        var options = System.Text.Json.JsonSerializer
            .Deserialize<TextEditorOptions>(optionsJsonString);

        if (options is null)
            return;
        
        if (options.Theme is not null)
        {
            var matchedTheme = _themeState.Value.ThemeRecordsList
                .FirstOrDefault(x =>
                    x.ThemeKey == options.Theme.ThemeKey);

            SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
        }
        
        if (options.KeymapDefinition is not null)
        {
            var matchedKeymapDefinition = KeymapFacts.AllKeymapDefinitions
                .FirstOrDefault(x =>
                    x.KeymapKey == options.KeymapDefinition.KeymapKey);
            
            SetKeymap(matchedKeymapDefinition ?? KeymapFacts.DefaultKeymapDefinition);
        }
        
        if (options.FontSizeInPixels is not null)
            SetFontSize(options.FontSizeInPixels.Value);
        
        if (options.CursorWidthInPixels is not null)
            SetCursorWidth(options.CursorWidthInPixels.Value);
        
        if (options.HeightInPixels is not null)
            SetHeight(options.HeightInPixels.Value);
        
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