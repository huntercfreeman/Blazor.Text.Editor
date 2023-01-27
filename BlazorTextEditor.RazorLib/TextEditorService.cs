using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.DialogNotification.Store.DialogCase;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Facts;
using BlazorALaCarte.Shared.Storage;
using BlazorALaCarte.Shared.Store.ThemeCase;
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
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Diff;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IState<TextEditorModelsCollection> _textEditorModelsCollectionWrap;
    private readonly IState<TextEditorViewModelsCollection> _textEditorViewModelsCollectionWrap;
    private readonly IState<TextEditorGroupsCollection> _textEditorGroupsCollectionWrap;
    private readonly IState<TextEditorDiffsCollection> _textEditorDiffsCollectionWrap;
    private readonly IState<ThemeState> _themeStateWrap;
    private readonly IState<TextEditorGlobalOptions> _textEditorGlobalOptionsWrap;
    private readonly IDispatcher _dispatcher;
    private readonly IStorageProvider _storageProvider;
    
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;

    public TextEditorService(
        IState<TextEditorModelsCollection> textEditorModelsCollectionWrap,
        IState<TextEditorViewModelsCollection> textEditorViewModelsCollectionWrap,
        IState<TextEditorGroupsCollection> textEditorGroupsCollectionWrap,
        IState<TextEditorDiffsCollection> textEditorDiffsCollectionWrap,
        IState<ThemeState> themeStateWrap,
        IState<TextEditorGlobalOptions> textEditorGlobalOptionsWrap,
        IDispatcher dispatcher,
        IStorageProvider storageProvider,
        IJSRuntime jsRuntime)
    {
        _textEditorModelsCollectionWrap = textEditorModelsCollectionWrap;
        _textEditorViewModelsCollectionWrap = textEditorViewModelsCollectionWrap;
        _textEditorGroupsCollectionWrap = textEditorGroupsCollectionWrap;
        _textEditorDiffsCollectionWrap = textEditorDiffsCollectionWrap;
        _themeStateWrap = themeStateWrap;
        _textEditorGlobalOptionsWrap = textEditorGlobalOptionsWrap;
        _dispatcher = dispatcher;
        _storageProvider = storageProvider;
        _jsRuntime = jsRuntime;

        _textEditorModelsCollectionWrap.StateChanged += ModelsCollectionWrapOnModelsCollectionWrapChanged;
        _textEditorViewModelsCollectionWrap.StateChanged += ViewModelsCollectionWrapOnStateChanged;
        _textEditorGroupsCollectionWrap.StateChanged += GroupsCollectionWrapOnStateChanged;
        _textEditorDiffsCollectionWrap.StateChanged += TextEditorDiffsCollectionWrapOnStateChanged;
        _textEditorGlobalOptionsWrap.StateChanged += GlobalOptionsWrapOnStateChanged;
    }

    public TextEditorModelsCollection TextEditorModelsCollection => _textEditorModelsCollectionWrap.Value;
    public TextEditorViewModelsCollection TextEditorViewModelsCollection => _textEditorViewModelsCollectionWrap.Value;
    public TextEditorGroupsCollection TextEditorGroupsCollection => _textEditorGroupsCollectionWrap.Value;
    public TextEditorDiffsCollection TextEditorDiffsCollection => _textEditorDiffsCollectionWrap.Value;
    public TextEditorGlobalOptions TextEditorGlobalOptions => _textEditorGlobalOptionsWrap.Value;
    
    public ThemeRecord? GlobalThemeValue => _textEditorGlobalOptionsWrap.Value.Options.Theme;
    public string GlobalThemeCssClassString => _textEditorGlobalOptionsWrap.Value.Options.Theme?.CssClassString ?? string.Empty;
    public string GlobalFontSizeInPixelsStyling => $"font-size: {_textEditorGlobalOptionsWrap.Value.Options.FontSizeInPixels!.Value.ToCssValue()}px;";
    public bool GlobalShowNewlines => _textEditorGlobalOptionsWrap.Value.Options.ShowNewlines!.Value;
    public bool GlobalShowWhitespace => _textEditorGlobalOptionsWrap.Value.Options.ShowWhitespace!.Value;
    public int GlobalFontSizeInPixelsValue => _textEditorGlobalOptionsWrap.Value.Options.FontSizeInPixels!.Value;
    public double GlobalCursorWidthInPixelsValue => _textEditorGlobalOptionsWrap.Value.Options.CursorWidthInPixels!.Value;
    public KeymapDefinition GlobalKeymapDefinition => _textEditorGlobalOptionsWrap.Value.Options.KeymapDefinition!;
    public int? GlobalHeightInPixelsValue => _textEditorGlobalOptionsWrap.Value.Options.HeightInPixels;

    public event Action? ModelsCollectionChanged;
    public event Action? ViewModelsCollectionChanged;
    public event Action? GroupsCollectionChanged;
    public event Action? DiffsCollectionChanged;
    public event Action? GlobalOptionsChanged;
        
    public void RegisterCustomTextEditorModel(
        TextEditorModel textEditorModel)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.RegisterAction(
                textEditorModel));
    }

    public void RegisterTemplatedTextEditorModel(
        TextEditorModelKey textEditorModelKey,
        WellKnownModelKind wellKnownModelKind,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent)
    {
        ILexer? lexer = null;
        IDecorationMapper? decorationMapper = null;
        
        switch (wellKnownModelKind)
        {
            case WellKnownModelKind.CSharp:
                lexer = new TextEditorCSharpLexer();
                decorationMapper = new TextEditorCSharpDecorationMapper();
                break;
            case WellKnownModelKind.Html:
                lexer = new TextEditorHtmlLexer();
                decorationMapper = new TextEditorHtmlDecorationMapper();
                break;
            case WellKnownModelKind.Css:
                lexer = new TextEditorCssLexer();
                decorationMapper = new TextEditorCssDecorationMapper();
                break;
            case WellKnownModelKind.Json:
                lexer = new TextEditorJsonLexer();
                decorationMapper = new TextEditorJsonDecorationMapper();
                break;
            case WellKnownModelKind.FSharp:
                lexer = new TextEditorFSharpLexer();
                decorationMapper = new TextEditorFSharpDecorationMapper();
                break;
            case WellKnownModelKind.Razor:
                lexer = new TextEditorRazorLexer();
                decorationMapper = new TextEditorHtmlDecorationMapper();
                break;
            case WellKnownModelKind.JavaScript:
                lexer = new TextEditorJavaScriptLexer();
                decorationMapper = new TextEditorJavaScriptDecorationMapper();
                break;
            case WellKnownModelKind.TypeScript:
                lexer = new TextEditorTypeScriptLexer();
                decorationMapper = new TextEditorTypeScriptDecorationMapper();
                break;
        }
        
        var textEditorModel = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            lexer,
            decorationMapper,
            null,
            textEditorModelKey);

        _ = Task.Run(async () =>
        {
            await textEditorModel.ApplySyntaxHighlightingAsync();
            
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.ForceRerenderAction(
                    textEditorModel.ModelKey));
        });
        
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.RegisterAction(
                textEditorModel));
    }

    public string? GetAllText(TextEditorModelKey textEditorModelKey)
    {
        return TextEditorModelsCollection.TextEditorList
            .FirstOrDefault(x => x.ModelKey == textEditorModelKey)
            ?.GetAllText();
    }
    
    public string? GetAllText(TextEditorViewModelKey textEditorViewModelKey)
    {
        var textEditorModel = GetTextEditorModelFromViewModelKey(textEditorViewModelKey);

        return textEditorModel is null 
            ? null 
            : GetAllText(textEditorModel.ModelKey);
    }

    public void InsertText(TextEditorModelsCollection.InsertTextAction insertTextAction)
    {
        _dispatcher.Dispatch(insertTextAction);
    }

    public void HandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction)
    {
        _dispatcher.Dispatch(keyboardEventAction);
    }
    
    public void DeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction)
    {
        _dispatcher.Dispatch(deleteTextByMotionAction);
    }
    
    public void DeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction)
    {
        _dispatcher.Dispatch(deleteTextByRangeAction);
    }
    
    public void RedoEdit(TextEditorModelKey textEditorModelKey)
    {
        var redoEditAction = new TextEditorModelsCollection.RedoEditAction(textEditorModelKey);
        
        _dispatcher.Dispatch(redoEditAction);
    }
    
    public void UndoEdit(TextEditorModelKey textEditorModelKey)
    {
        var undoEditAction = new TextEditorModelsCollection.UndoEditAction(textEditorModelKey);
        
        _dispatcher.Dispatch(undoEditAction);
    }

    public void DisposeTextEditor(TextEditorModelKey textEditorModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.DisposeAction(
                textEditorModelKey));
    }

    public void SetFontSize(int fontSizeInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetFontSizeAction(
                fontSizeInPixels));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }
    
    public void SetCursorWidth(double cursorWidthInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetCursorWidthAction(
                cursorWidthInPixels));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }
    
    public void SetHeight(int? heightInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetHeightAction(
                heightInPixels));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetTheme(ThemeRecord theme)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetThemeAction(
                theme));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }
    
    public void SetKeymap(KeymapDefinition foundKeymap)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetKeymapAction(
                foundKeymap));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetShowWhitespace(bool showWhitespace)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetShowWhitespaceAction(
                showWhitespace));
        
        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetShowNewlines(bool showNewlines)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetShowNewlinesAction(
                showNewlines));

        WriteGlobalTextEditorOptionsToLocalStorage();
    }

    public void SetUsingRowEndingKind(TextEditorModelKey textEditorModelKey, RowEndingKind rowEndingKind)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.SetUsingRowEndingKindAction(
                textEditorModelKey, 
                rowEndingKind));
    }
    
    public void SetResourceData(
        TextEditorModelKey textEditorModelKey,
        string resourceUri,
        DateTime resourceLastWriteTime)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.SetResourceDataAction(
                textEditorModelKey,
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
            new DialogRecordsCollection.RegisterAction(
                settingsDialog));
    }
    
    private void ModelsCollectionWrapOnModelsCollectionWrapChanged(object? sender, EventArgs e)
    {
        ModelsCollectionChanged?.Invoke();
    }
    
    private void ViewModelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        ViewModelsCollectionChanged?.Invoke();
    }

    private void GroupsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        GroupsCollectionChanged?.Invoke();
    }
    
    private void TextEditorDiffsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        DiffsCollectionChanged?.Invoke();
    }

    private void GlobalOptionsWrapOnStateChanged(object? sender, EventArgs e)
    {
        GlobalOptionsChanged?.Invoke();
    }

    public void RegisterGroup(TextEditorGroupKey textEditorGroupKey)
    {
        var textEditorGroup = new TextEditorGroup(
            textEditorGroupKey,
            TextEditorViewModelKey.Empty,
            ImmutableList<TextEditorViewModelKey>.Empty);
        
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.RegisterAction(
                textEditorGroup));
    }

    public void AddViewModelToGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.AddViewModelToGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }
    
    public void RemoveViewModelFromGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.RemoveViewModelFromGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }
    
    public void SetActiveViewModelOfGroup(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.SetActiveViewModelOfGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }

    public void RegisterViewModel(
        TextEditorViewModelKey textEditorViewModelKey,
        TextEditorModelKey textEditorModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorViewModelsCollection.RegisterAction(
                textEditorViewModelKey,
                textEditorModelKey, 
                this));
    }

    public ImmutableArray<TextEditorViewModel> GetViewModelsForModel(TextEditorModelKey textEditorModelKey)
    {
        return _textEditorViewModelsCollectionWrap.Value.ViewModelsList
            .Where(x => 
                x.ModelKey == textEditorModelKey)
            .ToImmutableArray();
    }

    public TextEditorModel? GetTextEditorModelFromViewModelKey(TextEditorViewModelKey textEditorViewModelKey)
    {
        var textEditorViewModelsCollection = _textEditorViewModelsCollectionWrap.Value;
        
        var viewModel = textEditorViewModelsCollection.ViewModelsList
            .FirstOrDefault(x => 
                x.ViewModelKey == textEditorViewModelKey);
        
        if (viewModel is null)
            return null;
        
        return GetTextEditorModelOrDefault(viewModel.ModelKey);
    }

    public void SetViewModelWith(
        TextEditorViewModelKey textEditorViewModelKey,
        Func<TextEditorViewModel, TextEditorViewModel> withFunc)
    {
        _dispatcher.Dispatch(
            new TextEditorViewModelsCollection.SetViewModelWithAction(
                textEditorViewModelKey,
                withFunc));
    }
    
    public void RegisterDiff(
        TextEditorDiffKey diffKey,
        TextEditorViewModelKey beforeViewModelKey,
        TextEditorViewModelKey afterViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorDiffsCollection.RegisterAction(
                diffKey,
                beforeViewModelKey,
                afterViewModelKey));
    }
    
    public void DisposeDiff(TextEditorDiffKey textEditorDiffKey)
    {
        _dispatcher.Dispatch(
            new TextEditorDiffsCollection.DisposeAction(
                textEditorDiffKey));
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
    
    public TextEditorModel? GetTextEditorModelOrDefaultByResourceUri(string resourceUri)
    {
        return TextEditorModelsCollection.TextEditorList
            .FirstOrDefault(x =>
                x.ResourceUri == resourceUri);
    }
    
    public void ReloadTextEditorModel(
        TextEditorModelKey textEditorModelKey,
        string content)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.ReloadAction(
                textEditorModelKey,
                content));
    }
    
    public TextEditorModel? GetTextEditorModelOrDefault(TextEditorModelKey textEditorModelKey)
    {
        return TextEditorModelsCollection.TextEditorList
            .FirstOrDefault(x =>
                x.ModelKey == textEditorModelKey);
    }
    
    public TextEditorViewModel? GetTextEditorViewModelOrDefault(TextEditorViewModelKey textEditorViewModelKey)
    {
        return _textEditorViewModelsCollectionWrap.Value.ViewModelsList
            .FirstOrDefault(x => 
                x.ViewModelKey == textEditorViewModelKey);
    }
    
    public TextEditorGroup? GetTextEditorGroupOrDefault(TextEditorGroupKey textEditorGroupKey)
    {
        return _textEditorGroupsCollectionWrap.Value.GroupsList
            .FirstOrDefault(x => 
                x.GroupKey == textEditorGroupKey);
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
            var matchedTheme = _themeStateWrap.Value.ThemeRecordsList
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
                _textEditorGlobalOptionsWrap.Value.Options));
    }
    
    public void Dispose()
    {
        _textEditorModelsCollectionWrap.StateChanged -= ModelsCollectionWrapOnModelsCollectionWrapChanged;
        _textEditorViewModelsCollectionWrap.StateChanged -= ViewModelsCollectionWrapOnStateChanged;
        _textEditorGroupsCollectionWrap.StateChanged -= GroupsCollectionWrapOnStateChanged;
        _textEditorDiffsCollectionWrap.StateChanged -= TextEditorDiffsCollectionWrapOnStateChanged;
        _textEditorGlobalOptionsWrap.StateChanged -= GlobalOptionsWrapOnStateChanged;
    }
}