using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Find;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    public IModelApi Model { get; }
    public IViewModelApi ViewModel { get; }
    public IGroupApi Group { get; }
    public IDiffApi Diff { get; }
    public IOptionsApi Options { get; }
    public IFindProviderApi FindProvider { get; }

    public IState<TextEditorModelsCollection> ModelsCollectionWrap { get; }
    public IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; }
    public IState<TextEditorGroupsCollection> GroupsCollectionWrap { get; }
    public IState<TextEditorDiffsCollection> DiffsCollectionWrap { get; }
    public IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; }
    public IState<TextEditorOptionsState> OptionsWrap { get; }
    public IState<TextEditorFindProviderState> FindProviderState { get; }
}