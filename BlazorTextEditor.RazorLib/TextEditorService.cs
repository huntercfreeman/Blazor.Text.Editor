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

    public TextEditorModelsCollection ModelsCollection => _textEditorModelsCollectionWrap.Value;
    public TextEditorViewModelsCollection ViewModelsCollection => _textEditorViewModelsCollectionWrap.Value;
    public TextEditorGroupsCollection GroupsCollection => _textEditorGroupsCollectionWrap.Value;
    public TextEditorDiffsCollection DiffsCollection => _textEditorDiffsCollectionWrap.Value;
    public TextEditorGlobalOptions GlobalOptions => _textEditorGlobalOptionsWrap.Value;
    
    public ThemeRecord? GlobalTheme => _textEditorGlobalOptionsWrap.Value.Options.Theme;
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
        
    public void ModelRegisterCustomModel(
        TextEditorModel model)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.RegisterAction(
                model));
    }

    public void ModelRegisterTemplatedModel(
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

    public string? ModelGetAllText(TextEditorModelKey textEditorModelKey)
    {
        return ModelsCollection.TextEditorList
            .FirstOrDefault(x => x.ModelKey == textEditorModelKey)
            ?.GetAllText();
    }
    
    public string? ViewModelGetAllText(TextEditorViewModelKey textEditorViewModelKey)
    {
        var textEditorModel = ViewModelGetModelOrDefault(textEditorViewModelKey);

        return textEditorModel is null 
            ? null 
            : ModelGetAllText(textEditorModel.ModelKey);
    }

    public void ModelInsertText(TextEditorModelsCollection.InsertTextAction insertTextAction)
    {
        _dispatcher.Dispatch(insertTextAction);
    }

    public void ModelHandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction)
    {
        _dispatcher.Dispatch(keyboardEventAction);
    }
    
    public void ModelDeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction)
    {
        _dispatcher.Dispatch(deleteTextByMotionAction);
    }
    
    public void ModelDeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction)
    {
        _dispatcher.Dispatch(deleteTextByRangeAction);
    }
    
    public void ModelRedoEdit(TextEditorModelKey textEditorModelKey)
    {
        var redoEditAction = new TextEditorModelsCollection.RedoEditAction(textEditorModelKey);
        
        _dispatcher.Dispatch(redoEditAction);
    }
    
    public void ModelUndoEdit(TextEditorModelKey textEditorModelKey)
    {
        var undoEditAction = new TextEditorModelsCollection.UndoEditAction(textEditorModelKey);
        
        _dispatcher.Dispatch(undoEditAction);
    }

    public void ModelDispose(TextEditorModelKey textEditorModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.DisposeAction(
                textEditorModelKey));
    }

    public void GlobalOptionsSetFontSize(int fontSizeInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetFontSizeAction(
                fontSizeInPixels));
        
        GlobalOptionsWriteToLocalStorage();
    }
    
    public void GlobalOptionsSetCursorWidth(double cursorWidthInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetCursorWidthAction(
                cursorWidthInPixels));
        
        GlobalOptionsWriteToLocalStorage();
    }
    
    public void GlobalOptionsSetHeight(int? heightInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetHeightAction(
                heightInPixels));
        
        GlobalOptionsWriteToLocalStorage();
    }

    public void GlobalOptionsSetTheme(ThemeRecord theme)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetThemeAction(
                theme));
        
        GlobalOptionsWriteToLocalStorage();
    }
    
    public void GlobalOptionsSetKeymap(KeymapDefinition foundKeymap)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetKeymapAction(
                foundKeymap));
        
        GlobalOptionsWriteToLocalStorage();
    }

    public void GlobalOptionsSetShowWhitespace(bool showWhitespace)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetShowWhitespaceAction(
                showWhitespace));
        
        GlobalOptionsWriteToLocalStorage();
    }

    public void GlobalOptionsSetShowNewlines(bool showNewlines)
    {
        _dispatcher.Dispatch(
            new TextEditorGlobalOptions.SetShowNewlinesAction(
                showNewlines));

        GlobalOptionsWriteToLocalStorage();
    }

    public void ModelSetUsingRowEndingKind(TextEditorModelKey textEditorModelKey, RowEndingKind rowEndingKind)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.SetUsingRowEndingKindAction(
                textEditorModelKey, 
                rowEndingKind));
    }
    
    public void ModelSetResourceData(
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

    public void GlobalOptionsShowSettingsDialog(bool isResizable = false)
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

    public void GroupRegister(TextEditorGroupKey textEditorGroupKey)
    {
        var textEditorGroup = new TextEditorGroup(
            textEditorGroupKey,
            TextEditorViewModelKey.Empty,
            ImmutableList<TextEditorViewModelKey>.Empty);
        
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.RegisterAction(
                textEditorGroup));
    }

    public void GroupAddViewModel(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.AddViewModelToGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }
    
    public void GroupRemoveViewModel(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.RemoveViewModelFromGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }
    
    public void GroupSetActiveViewModel(
        TextEditorGroupKey textEditorGroupKey,
        TextEditorViewModelKey textEditorViewModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorGroupsCollection.SetActiveViewModelOfGroupAction(
                textEditorGroupKey,
                textEditorViewModelKey));
    }

    public void ViewModelRegister(
        TextEditorViewModelKey textEditorViewModelKey,
        TextEditorModelKey textEditorModelKey)
    {
        _dispatcher.Dispatch(
            new TextEditorViewModelsCollection.RegisterAction(
                textEditorViewModelKey,
                textEditorModelKey, 
                this));
    }

    public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(TextEditorModelKey textEditorModelKey)
    {
        return _textEditorViewModelsCollectionWrap.Value.ViewModelsList
            .Where(x => 
                x.ModelKey == textEditorModelKey)
            .ToImmutableArray();
    }

    public TextEditorModel? ViewModelGetModelOrDefault(TextEditorViewModelKey textEditorViewModelKey)
    {
        var textEditorViewModelsCollection = _textEditorViewModelsCollectionWrap.Value;
        
        var viewModel = textEditorViewModelsCollection.ViewModelsList
            .FirstOrDefault(x => 
                x.ViewModelKey == textEditorViewModelKey);
        
        if (viewModel is null)
            return null;
        
        return ModelFindOrDefault(viewModel.ModelKey);
    }

    public void ViewModelWith(
        TextEditorViewModelKey textEditorViewModelKey,
        Func<TextEditorViewModel, TextEditorViewModel> withFunc)
    {
        _dispatcher.Dispatch(
            new TextEditorViewModelsCollection.SetViewModelWithAction(
                textEditorViewModelKey,
                withFunc));
    }
    
    public void DiffRegister(
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
    
    public void DiffDispose(TextEditorDiffKey textEditorDiffKey)
    {
        _dispatcher.Dispatch(
            new TextEditorDiffsCollection.DisposeAction(
                textEditorDiffKey));
    }
    
    public async Task ViewModelSetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.setGutterScrollTop",
            gutterElementId,
            scrollTopInPixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }

    public async Task ViewModelMutateScrollHorizontalPositionAsync(
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
    
    public async Task ViewModelMutateScrollVerticalPositionAsync(
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
    public async Task ViewModelSetScrollPositionAsync(
        string bodyElementId,
        string gutterElementId,
        double? scrollLeftInPixels,
        double? scrollTopInPixels)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.setScrollPosition",
            bodyElementId,
            gutterElementId,
            scrollLeftInPixels,
            scrollTopInPixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
    }

    public async Task<ElementMeasurementsInPixels> ElementMeasurementsInPixelsAsync(string elementId)
    {
        return await _jsRuntime.InvokeAsync<ElementMeasurementsInPixels>(
            "blazorTextEditor.getElementMeasurementsInPixelsById",
            elementId);
    }
    
    public TextEditorModel? ResourceUriGetModelOrDefault(string resourceUri)
    {
        return ModelsCollection.TextEditorList
            .FirstOrDefault(x =>
                x.ResourceUri == resourceUri);
    }
    
    public void ModelReload(
        TextEditorModelKey textEditorModelKey,
        string content)
    {
        _dispatcher.Dispatch(
            new TextEditorModelsCollection.ReloadAction(
                textEditorModelKey,
                content));
    }
    
    public TextEditorModel? ModelFindOrDefault(TextEditorModelKey textEditorModelKey)
    {
        return ModelsCollection.TextEditorList
            .FirstOrDefault(x =>
                x.ModelKey == textEditorModelKey);
    }
    
    public TextEditorViewModel? ViewModelFindOrDefault(TextEditorViewModelKey textEditorViewModelKey)
    {
        return _textEditorViewModelsCollectionWrap.Value.ViewModelsList
            .FirstOrDefault(x => 
                x.ViewModelKey == textEditorViewModelKey);
    }
    
    public TextEditorGroup? GroupFindOrDefault(TextEditorGroupKey textEditorGroupKey)
    {
        return _textEditorGroupsCollectionWrap.Value.GroupsList
            .FirstOrDefault(x => 
                x.GroupKey == textEditorGroupKey);
    }
    
    public async Task CursorPrimaryFocusAsync(string primaryCursorContentId)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.focusHtmlElementById",
            primaryCursorContentId);
    }
    
    public async Task GlobalOptionsSetFromLocalStorageAsync()
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

            GlobalOptionsSetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
        }
        
        if (options.KeymapDefinition is not null)
        {
            var matchedKeymapDefinition = KeymapFacts.AllKeymapDefinitions
                .FirstOrDefault(x =>
                    x.KeymapKey == options.KeymapDefinition.KeymapKey);
            
            GlobalOptionsSetKeymap(matchedKeymapDefinition ?? KeymapFacts.DefaultKeymapDefinition);
        }
        
        if (options.FontSizeInPixels is not null)
            GlobalOptionsSetFontSize(options.FontSizeInPixels.Value);
        
        if (options.CursorWidthInPixels is not null)
            GlobalOptionsSetCursorWidth(options.CursorWidthInPixels.Value);
        
        if (options.HeightInPixels is not null)
            GlobalOptionsSetHeight(options.HeightInPixels.Value);
        
        if (options.ShowNewlines is not null)
            GlobalOptionsSetShowNewlines(options.ShowNewlines.Value);
        
        if (options.ShowWhitespace is not null)
            GlobalOptionsSetShowWhitespace(options.ShowWhitespace.Value);
    }
    
    public void GlobalOptionsWriteToLocalStorage()
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