using System.Collections.Immutable;
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.DialogCase;
using BlazorCommon.RazorLib.Store.StorageCase;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Options;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IDispatcher _dispatcher;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IStorageService _storageService;

    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;

    public TextEditorService(
        IState<TextEditorModelsCollection> modelsCollectionWrap,
        IState<TextEditorViewModelsCollection> viewModelsCollectionWrap,
        IState<TextEditorGroupsCollection> groupsCollectionWrap,
        IState<TextEditorDiffsCollection> diffsCollectionWrap,
        IState<ThemeRecordsCollection> themeRecordsCollectionWrap,
        IState<TextEditorOptionsState> textEditorOptionsWrap,
        IDispatcher dispatcher,
        IBackgroundTaskQueue backgroundTaskQueue,
        IStorageService storageService,
        IJSRuntime jsRuntime)
    {
        ModelsCollectionWrap = modelsCollectionWrap;
        ViewModelsCollectionWrap = viewModelsCollectionWrap;
        GroupsCollectionWrap = groupsCollectionWrap;
        DiffsCollectionWrap = diffsCollectionWrap;
        ThemeRecordsCollectionWrap = themeRecordsCollectionWrap;
        OptionsWrap = textEditorOptionsWrap;
        _dispatcher = dispatcher;
        _backgroundTaskQueue = backgroundTaskQueue;
        _storageService = storageService;
        _jsRuntime = jsRuntime;

        Model = new ITextEditorService.ModelApi(
            _dispatcher,
            this);

        ViewModel = new ITextEditorService.ViewModelApi(
            _dispatcher,
            _jsRuntime,
            this);

        Group = new ITextEditorService.GroupApi(
            _dispatcher,
            this);
    }
    
    public IState<TextEditorModelsCollection> ModelsCollectionWrap { get; }
    public IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; }
    public IState<TextEditorGroupsCollection> GroupsCollectionWrap { get; }
    public IState<TextEditorDiffsCollection> DiffsCollectionWrap { get; }
    public IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; }
    public IState<TextEditorOptionsState> OptionsWrap { get; }
    public string StorageKey => "bte_text-editor-options";
    
    public string ThemeCssClassString => ThemeRecordsCollectionWrap.Value.ThemeRecordsList
                                                   .FirstOrDefault(x =>
                                                       x.ThemeKey == OptionsWrap.Value.Options.CommonOptions
                                                           .ThemeKey)
                                                   ?.CssClassString
                                               ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public ITextEditorService.IModelApi Model { get; }
    public ITextEditorService.IViewModelApi ViewModel { get; }
    public ITextEditorService.IGroupApi Group { get; }

    public void OptionsSetFontFamily(string? fontFamily)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetFontFamilyAction(
                fontFamily));
        
        OptionsWriteToStorage();
    }

    public void OptionsSetFontSize(int fontSizeInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetFontSizeAction(
                fontSizeInPixels));
        
        OptionsWriteToStorage();
    }
    
    public void OptionsSetCursorWidth(double cursorWidthInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetCursorWidthAction(
                cursorWidthInPixels));
        
        OptionsWriteToStorage();
    }
    
    public void OptionsSetHeight(int? heightInPixels)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetHeightAction(
                heightInPixels));
        
        OptionsWriteToStorage();
    }

    public void OptionsSetTheme(ThemeRecord theme)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetThemeAction(
                theme));
        
        OptionsWriteToStorage();
    }
    
    public void OptionsSetKeymap(KeymapDefinition foundKeymap)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetKeymapAction(
                foundKeymap));
        
        OptionsWriteToStorage();
    }

    public void OptionsSetShowWhitespace(bool showWhitespace)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetShowWhitespaceAction(
                showWhitespace));
        
        OptionsWriteToStorage();
    }

    public void OptionsSetShowNewlines(bool showNewlines)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetShowNewlinesAction(
                showNewlines));

        OptionsWriteToStorage();
    }
    
    public void OptionsSetUseMonospaceOptimizations(bool useMonospaceOptimizations)
    {
        _dispatcher.Dispatch(
            new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(
                useMonospaceOptimizations));

        OptionsWriteToStorage();
    }

    public void OptionsShowSettingsDialog(
        bool isResizable = false,
        string? cssClassString = null)
    {
        var settingsDialog = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Text Editor Settings",
            typeof(TextEditorSettings),
            null,
            cssClassString)
        {
            IsResizable = isResizable
        };
        
        _dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                settingsDialog));
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
    
    public TextEditorDiffResult? DiffCalculate(TextEditorDiffKey textEditorDiffKey,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        
        var textEditorDiff = DiffModelFindOrDefault(textEditorDiffKey);

        if (textEditorDiff is null)
            return null;
        
        var beforeViewModel = ViewModel
            .FindOrDefault(textEditorDiff.BeforeViewModelKey);

        var afterViewModel = ViewModel
            .FindOrDefault(textEditorDiff.AfterViewModelKey);

        if (beforeViewModel is null ||
            afterViewModel is null)
        {
            return null;
        }
        
        var beforeModel = Model.ModelFindOrDefault(beforeViewModel.ModelKey);
        var afterModel = Model.ModelFindOrDefault(afterViewModel.ModelKey);

        if (beforeModel is null ||
            afterModel is null)
        {
            return null;
        }
        
        var beforeText = beforeModel.GetAllText();
        var afterText = afterModel.GetAllText();

        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        ChangeFirstPresentationLayer(
            beforeViewModel.ViewModelKey,
            diffResult.BeforeLongestCommonSubsequenceTextSpans);
        
        ChangeFirstPresentationLayer(
            afterViewModel.ViewModelKey,
            diffResult.AfterLongestCommonSubsequenceTextSpans);

        return diffResult;
        
        void ChangeFirstPresentationLayer(
            TextEditorViewModelKey viewModelKey,
            ImmutableList<TextEditorTextSpan> longestCommonSubsequenceTextSpans)
        {
            _dispatcher.Dispatch(
                new TextEditorViewModelsCollection.SetViewModelWithAction(
                    viewModelKey,
                    inViewModel =>
                    {
                        var outPresentationLayer = inViewModel.FirstPresentationLayer;
                    
                        var inPresentationModel = outPresentationLayer
                            .FirstOrDefault(x =>
                                x.TextEditorPresentationKey == DiffFacts.PresentationKey);

                        if (inPresentationModel is null)
                        {
                            inPresentationModel = DiffFacts.EmptyPresentationModel;
                        
                            outPresentationLayer = outPresentationLayer.Add(
                                inPresentationModel);
                        }

                        var outPresentationModel = inPresentationModel with
                        {
                            TextEditorTextSpans = longestCommonSubsequenceTextSpans
                        };
                    
                        outPresentationLayer = outPresentationLayer.Replace(
                            inPresentationModel,
                            outPresentationModel);

                        return inViewModel with
                        {
                            FirstPresentationLayer = outPresentationLayer,
                            TextEditorStateChangedKey = TextEditorStateChangedKey.NewTextEditorStateChangedKey()
                        };
                    }));
        }
    }
    
    public void DiffDispose(
        TextEditorDiffKey textEditorDiffKey)
    {
        _dispatcher.Dispatch(
            new TextEditorDiffsCollection.DisposeAction(
                textEditorDiffKey));
    }
    
    public TextEditorDiffModel? DiffModelFindOrDefault(
        TextEditorDiffKey textEditorDiffKey)
    {
        return DiffsCollectionWrap.Value.DiffModelsList
            .FirstOrDefault(x =>
                x.DiffKey == textEditorDiffKey);
    }

    public async Task<ElementMeasurementsInPixels> ElementMeasurementsInPixelsAsync(
        string elementId)
    {
        return await _jsRuntime.InvokeAsync<ElementMeasurementsInPixels>(
            "blazorTextEditor.getElementMeasurementsInPixelsById",
            elementId);
    }
    
    public async Task CursorPrimaryFocusAsync(string primaryCursorContentId)
    {
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.focusHtmlElementById",
            primaryCursorContentId);
    }
    
    public async Task OptionsSetFromLocalStorageAsync()
    {
        var optionsJsonString = await _storageService
                .GetValue(StorageKey)
                as string;

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;
        
        var options = System.Text.Json.JsonSerializer
            .Deserialize<Options.TextEditorOptions>(optionsJsonString);

        if (options is null)
            return;
        
        if (options.CommonOptions?.ThemeKey is not null)
        {
            var matchedTheme = ThemeRecordsCollectionWrap.Value.ThemeRecordsList
                .FirstOrDefault(x =>
                    x.ThemeKey == options.CommonOptions.ThemeKey);

            OptionsSetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
        }
        
        if (options.KeymapDefinition is not null)
        {
            var matchedKeymapDefinition = KeymapFacts.AllKeymapDefinitions
                .FirstOrDefault(x =>
                    x.KeymapKey == options.KeymapDefinition.KeymapKey);
            
            OptionsSetKeymap(matchedKeymapDefinition ?? KeymapFacts.DefaultKeymapDefinition);
        }
        
        if (options.CommonOptions?.FontSizeInPixels is not null)
            OptionsSetFontSize(options.CommonOptions.FontSizeInPixels.Value);
        
        if (options.CursorWidthInPixels is not null)
            OptionsSetCursorWidth(options.CursorWidthInPixels.Value);
        
        if (options.TextEditorHeightInPixels is not null)
            OptionsSetHeight(options.TextEditorHeightInPixels.Value);
        
        if (options.ShowNewlines is not null)
            OptionsSetShowNewlines(options.ShowNewlines.Value);
        
        // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
        // for a first time user. This leads to a bad user experience since the proportional
        // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
        // from local storage.
        //
        // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);
        
        if (options.ShowWhitespace is not null)
            OptionsSetShowWhitespace(options.ShowWhitespace.Value);
    }
    
    public void OptionsWriteToStorage()
    {
        _dispatcher.Dispatch(
            new StorageEffects.WriteToStorageAction(
                StorageKey,
                OptionsWrap.Value.Options));
    }
}