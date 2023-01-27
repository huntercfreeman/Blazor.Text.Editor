namespace BlazorTextEditor.Demo.ClassLib.TestDataFolder;

public static partial class TestData
{
    public static class CSharp
    {
        public const string EXAMPLE_TEXT_8_LINES = @"namespace BlazorTreeView.RazorLib;
public class MyClass
{
    public void MyMethod()
    {
        return;
    }
}";
        public const string DIFF_DEMO_BEFORE_TEXT = @"namespace BlazorTreeView.RazorLib;
public class MyClass
{
    public void MyMethod()
    {
        return;
    }
}";
        
        public const string DIFF_DEMO_AFTER_TEXT = @"namespace BlazorTreeView.RazorLib;
public record MyRecord
{
    private static void AnotherMethod()
    {
        return;
    }

    public void MyMethod()
    {
        return;
    }
}";
        
        public const string EXAMPLE_TEXT_173_LINES = @"using System.Collections.Immutable;
using BlazorTreeView.RazorLib.Store.TreeViewCase;
using Fluxor;

namespace BlazorTreeView.RazorLib;

public class TreeViewService : ITreeViewService
{
    private readonly IDispatcher _dispatcher;
    private readonly IState<TreeViewStateContainer> _treeViewStateContainerWrap;

    public TreeViewService(
        IState<TreeViewStateContainer> treeViewStateContainer,
        IDispatcher dispatcher)
    {
        _treeViewStateContainerWrap = treeViewStateContainer;
        _dispatcher = dispatcher;

        _treeViewStateContainerWrap.StateChanged += TreeViewStateContainerOnStateChanged;
    }

    public TreeViewStateContainer TreeViewStateContainer => 
        _treeViewStateContainerWrap.Value;
    
    public ImmutableArray<TreeViewState> TreeViewStates =>
        _treeViewStateContainerWrap.Value.TreeViewStatesMap.Values.ToImmutableArray();
    
    public event Action? OnTreeViewStateContainerChanged;
    
    public void RegisterTreeViewState(TreeViewState treeViewState)
    {
        var registerTreeViewStateAction = new TreeViewStateContainer.RegisterTreeViewStateAction(
            treeViewState);
        
        _dispatcher.Dispatch(registerTreeViewStateAction);
    }
    
    public void ReplaceTreeViewState(
        TreeViewStateKey treeViewStateKey, 
        TreeViewState treeViewState)
    {
        var replaceTreeViewStateAction = new TreeViewStateContainer.ReplaceTreeViewStateAction(
            treeViewStateKey,
            treeViewState);
        
        _dispatcher.Dispatch(replaceTreeViewStateAction);
    }

    public void SetRoot(TreeViewStateKey treeViewStateKey, TreeView treeView)
    {
        var withRootAction = new TreeViewStateContainer.WithRootAction(
            treeViewStateKey, 
            treeView);
        
        _dispatcher.Dispatch(withRootAction);
    }

    public bool TryGetTreeViewState(
        TreeViewStateKey treeViewStateKey,
        out TreeViewState? treeViewState)
    {
        return _treeViewStateContainerWrap.Value.TreeViewStatesMap
            .TryGetValue(treeViewStateKey, out treeViewState);
    }

    public void ReRenderNode(
        TreeViewStateKey treeViewStateKey,
        TreeView node)
    {
        var replaceNodeAction = new TreeViewStateContainer.ReRenderNodeAction(
            treeViewStateKey,
            node);
        
        _dispatcher.Dispatch(replaceNodeAction);
    }

    public void AddChildNode(
        TreeViewStateKey treeViewStateKey,
        TreeView parent,
        TreeView child)
    {
        var addChildNodeAction = new TreeViewStateContainer.AddChildNodeAction(
            treeViewStateKey,
            parent,
            child);
        
        _dispatcher.Dispatch(addChildNodeAction);
    }

    public void SetActiveNode(
        TreeViewStateKey treeViewStateKey,
        TreeView nextActiveNode)
    {
        var setActiveNodeAction = new TreeViewStateContainer.SetActiveNodeAction(
            treeViewStateKey,
            nextActiveNode);
        
        _dispatcher.Dispatch(setActiveNodeAction);
    }

    public void MoveActiveSelectionLeft(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionLeftAction = new TreeViewStateContainer.MoveActiveSelectionLeftAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionLeftAction);
    }

    public void MoveActiveSelectionDown(
        TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionDownAction = new TreeViewStateContainer.MoveActiveSelectionDownAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionDownAction);
    }

    public void MoveActiveSelectionUp(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionUpAction = new TreeViewStateContainer.MoveActiveSelectionUpAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionUpAction);
    }

    public void MoveActiveSelectionRight(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionRightAction = new TreeViewStateContainer.MoveActiveSelectionRightAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionRightAction);
    }

    public void MoveActiveSelectionHome(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionHomeAction = new TreeViewStateContainer.MoveActiveSelectionHomeAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionHomeAction);
    }

    public void MoveActiveSelectionEnd(TreeViewStateKey treeViewStateKey)
    {
        var moveActiveSelectionEndAction = new TreeViewStateContainer.MoveActiveSelectionEndAction(
            treeViewStateKey);

        _dispatcher.Dispatch(moveActiveSelectionEndAction);
    }

    public string GetNodeElementId(
        TreeView treeView)
    {
        return $""btv_node-{treeView.Id}"";
    }

    public void DisposeTreeViewState(TreeViewStateKey treeViewStateKey)
    {
        var disposeTreeViewStateAction = new TreeViewStateContainer.DisposeTreeViewStateAction(
            treeViewStateKey);
        
        _dispatcher.Dispatch(disposeTreeViewStateAction);
    }
    
    private void TreeViewStateContainerOnStateChanged(object? sender, EventArgs e)
    {
        OnTreeViewStateContainerChanged?.Invoke();
    }
    
    public void Dispose()
    {
        _treeViewStateContainerWrap.StateChanged -= TreeViewStateContainerOnStateChanged;
    }
}";
        
        public const string TEXT_EDITOR_SERVICE_API = @"// Usings were omitted for conciseness

namespace BlazorTextEditor.RazorLib;

/// <summary>Methods are prepended by the State which is most meaningfully involved when invoking the method.<br/><br/>Example, the method ""ModelRedoEdit(...)"" will most meaningfully involve the <see cref=""TextEditorModel""/>. Therefore it starts with the word ""Model"".<br/><br/>The ""TextEditor"" part of ""TextEditorModel"" is left off because the method is scoped to the <see cref=""ITextEditorService""/>. Therefore, we can assume that ""Model"" refers to ""TextEditorModel"" unless explicitly stated otherwise.</summary>
public interface ITextEditorService : IDisposable
{
    #region ConstantsSortedAlphabetically

    /// <summary>Used as the key storing the <see cref=""GlobalOptions""/> using JavaScript and local storage.</summary>
    public const string LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY = ""bte_text-editor-options"";

    #endregion

    #region PropertiesSortedAlphabetically

    public double GlobalCursorWidthInPixelsValue { get; }
    public string GlobalFontSizeInPixelsStyling { get; }
    public int GlobalFontSizeInPixelsValue { get; }
    public int? GlobalHeightInPixelsValue { get; }
    public KeymapDefinition GlobalKeymapDefinition { get; }
    public bool GlobalShowNewlines { get; }
    public bool GlobalShowWhitespace { get; }
    public string GlobalThemeCssClassString { get; }
    public ThemeRecord? GlobalTheme { get; }
    /// <summary>Contains all registered <see cref=""TextEditorDiff""/></summary>
    public TextEditorDiffsCollection DiffsCollection { get; }
    public TextEditorGlobalOptions GlobalOptions { get; }
    /// <summary>Contains all registered <see cref=""TextEditorGroup""/></summary>
    public TextEditorGroupsCollection GroupsCollection { get; }
    /// <summary>Contains all registered <see cref=""TextEditorModel""/></summary>
    public TextEditorModelsCollection ModelsCollection { get; }
    /// <summary>Contains all registered <see cref=""TextEditorViewModel""/></summary>
    public TextEditorViewModelsCollection ViewModelsCollection { get; }

    #endregion

    #region EventsSortedAlphabetically

    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref=""TextEditorDiff""/><br/>-On dispose of a <see cref=""TextEditorDiff""/><br/>-On replacement of an immutable <see cref=""TextEditorDiff""/> which is contained within the <see cref=""TextEditorDiffsCollection""/></summary>
    public event Action? DiffsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On replacement of the immutable <see cref=""TextEditorOptions""/> which is contained within <see cref=""TextEditorGlobalOptions""/></summary>
    public event Action? GlobalOptionsChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref=""TextEditorGroup""/><br/>-On dispose of a <see cref=""TextEditorGroup""/><br/>-On replacement of an immutable <see cref=""TextEditorGroup""/> which is contained within the <see cref=""TextEditorGroupsCollection""/></summary>
    public event Action? GroupsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref=""TextEditorModel""/><br/>-On dispose of a <see cref=""TextEditorModel""/><br/>-On replacement of an immutable <see cref=""TextEditorModel""/> which is contained within the <see cref=""TextEditorModelsCollection""/></summary>
    public event Action? ModelsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref=""TextEditorViewModel""/><br/>-On dispose of a <see cref=""TextEditorViewModel""/><br/>-On replacement of an immutable <see cref=""TextEditorViewModel""/> which is contained within the <see cref=""TextEditorViewModelsCollection""/></summary>
    public event Action? ViewModelsCollectionChanged;

    #endregion

    #region MethodsSortedAlphabetically
    
    public Task CursorPrimaryFocusAsync(string primaryCursorContentId);
    public void DiffDispose(TextEditorDiffKey textEditorDiffKey);
    public void DiffRegister(TextEditorDiffKey diffKey, TextEditorViewModelKey beforeViewModelKey, TextEditorViewModelKey afterViewModelKey);
    public Task<ElementMeasurementsInPixels> ElementMeasurementsInPixelsAsync(string elementId);
    public void GlobalOptionsSetCursorWidth(double cursorWidthInPixels);
    public void GlobalOptionsSetFontSize(int fontSizeInPixels);
    public Task GlobalOptionsSetFromLocalStorageAsync();
    public void GlobalOptionsSetHeight(int? heightInPixels);
    public void GlobalOptionsSetKeymap(KeymapDefinition foundKeymap);
    public void GlobalOptionsSetShowNewlines(bool showNewlines);
    public void GlobalOptionsSetShowWhitespace(bool showWhitespace);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the ""BlazorALaCarte.Shared"" Themes which get applied at an application level. <br /><br /> This allows for a ""DarkTheme-Application"" that has a ""LightTheme-TextEditor""</summary>
    public void GlobalOptionsSetTheme(ThemeRecord theme);
    public void GlobalOptionsShowSettingsDialog(bool isResizable = false);
    public void GlobalOptionsWriteToLocalStorage();
    public void GroupAddViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public TextEditorGroup? GroupFindOrDefault(TextEditorGroupKey textEditorGroupKey);
    public void GroupRegister(TextEditorGroupKey textEditorGroupKey);
    public void GroupRemoveViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void GroupSetActiveViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void ModelDeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction);
    public void ModelDeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction);
    public void ModelDispose(TextEditorModelKey textEditorModelKey);
    public TextEditorModel? ModelFindOrDefault(TextEditorModelKey textEditorModelKey);
    public string? ModelGetAllText(TextEditorModelKey textEditorModelKey);
    public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(TextEditorModelKey textEditorModelKey);
    public void ModelHandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction);
    public void ModelInsertText(TextEditorModelsCollection.InsertTextAction insertTextAction);
    public void ModelRedoEdit(TextEditorModelKey textEditorModelKey);
    /// <summary>It is recommended to use the <see cref=""ModelRegisterTemplatedModel"" /> method as it will internally reference the <see cref=""ILexer"" /> and <see cref=""IDecorationMapper"" /> that correspond to the desired text editor.</summary>
    public void ModelRegisterCustomModel(TextEditorModel model);
    /// <summary>As an example, for a C# Text Editor one would pass in a <see cref=""WellKnownModelKind"" /> of <see cref=""WellKnownModelKind.CSharp"" />.<br /><br />For a Plain Text Editor one would pass in a <see cref=""WellKnownModelKind"" /> of <see cref=""WellKnownModelKind.Plain"" />.</summary>
    public void ModelRegisterTemplatedModel(TextEditorModelKey textEditorModelKey, WellKnownModelKind wellKnownModelKind, string resourceUri, DateTime resourceLastWriteTime, string fileExtension, string initialContent);
    public void ModelReload(TextEditorModelKey textEditorModelKey, string content);
    public void ModelSetResourceData(TextEditorModelKey textEditorModelKey, string resourceUri, DateTime resourceLastWriteTime);
    public void ModelSetUsingRowEndingKind(TextEditorModelKey textEditorModelKey, RowEndingKind rowEndingKind);
    public void ModelUndoEdit(TextEditorModelKey textEditorModelKey);
    public TextEditorModel? ResourceUriGetModelOrDefault(string resourceUri);
    public TextEditorViewModel? ViewModelFindOrDefault(TextEditorViewModelKey textEditorViewModelKey);
    public string? ViewModelGetAllText(TextEditorViewModelKey textEditorViewModelKey);
    public TextEditorModel? ViewModelGetModelOrDefault(TextEditorViewModelKey textEditorViewModelKey);
    public Task ViewModelMutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task ViewModelMutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
    public void ViewModelRegister(TextEditorViewModelKey textEditorViewModelKey, TextEditorModelKey textEditorModelKey);
    public Task ViewModelSetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels);
    public Task ViewModelSetScrollPositionAsync(string bodyElementId, string gutterElementId, double? scrollLeftInPixels, double? scrollTopInPixels);
    public void ViewModelWith(TextEditorViewModelKey textEditorViewModelKey, Func<TextEditorViewModel, TextEditorViewModel> withFunc);

    #endregion
}";
    }
}