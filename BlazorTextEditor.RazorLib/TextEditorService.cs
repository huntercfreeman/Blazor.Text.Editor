using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
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
        
        Diff = new ITextEditorService.DiffApi(
            _dispatcher,
            this);

        Options = new ITextEditorService.OptionsApi(
            _dispatcher,
            _storageService,
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
    public ITextEditorService.IDiffApi Diff { get; }
    public ITextEditorService.IOptionsApi Options { get; }
}