using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Find;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using Fluxor;
using Microsoft.JSInterop;
using static BlazorTextEditor.RazorLib.ITextEditorService;

namespace BlazorTextEditor.RazorLib;

public class TextEditorService : ITextEditorService
{
    private readonly IDispatcher _dispatcher;
    private readonly BlazorTextEditorOptions _blazorTextEditorOptions;
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
        IState<TextEditorFindProvidersCollection> textEditorFindProvidersCollectionWrap,
        IDispatcher dispatcher,
        BlazorTextEditorOptions blazorTextEditorOptions,
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
        TextEditorFindProvidersCollectionWrap = textEditorFindProvidersCollectionWrap;

        _dispatcher = dispatcher;
        _blazorTextEditorOptions = blazorTextEditorOptions;
        _backgroundTaskQueue = backgroundTaskQueue;
        _storageService = storageService;
        _jsRuntime = jsRuntime;

        Model = new ModelApi(
            _dispatcher,
            _blazorTextEditorOptions,
            this);

        ViewModel = new ViewModelApi(
            _dispatcher,
            _blazorTextEditorOptions,
            _jsRuntime,
            this);

        Group = new GroupApi(
            _dispatcher,
            _blazorTextEditorOptions,
            this);
        
        Diff = new DiffApi(
            _dispatcher,
            _blazorTextEditorOptions,
            this);

        Options = new OptionsApi(
            _dispatcher,
            _blazorTextEditorOptions,
            _storageService,
            this);

        FindProvider = new FindProviderApi(
            _dispatcher,
            _blazorTextEditorOptions,
            this);
    }
    
    public IState<TextEditorModelsCollection> ModelsCollectionWrap { get; }
    public IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; }
    public IState<TextEditorGroupsCollection> GroupsCollectionWrap { get; }
    public IState<TextEditorDiffsCollection> DiffsCollectionWrap { get; }
    public IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; }
    public IState<TextEditorOptionsState> OptionsWrap { get; }
    public IState<TextEditorFindProvidersCollection> TextEditorFindProvidersCollectionWrap { get; }

    public string StorageKey => "bte_text-editor-options";
    
    public string ThemeCssClassString => ThemeRecordsCollectionWrap.Value.ThemeRecordsList
            .FirstOrDefault(x =>
                x.ThemeKey == OptionsWrap.Value.Options.CommonOptions
                    .ThemeKey)
            ?.CssClassString
        ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public IModelApi Model { get; }
    public IViewModelApi ViewModel { get; }
    public IGroupApi Group { get; }
    public IDiffApi Diff { get; }
    public IOptionsApi Options { get; }
    public IFindProviderApi FindProvider { get; }
}