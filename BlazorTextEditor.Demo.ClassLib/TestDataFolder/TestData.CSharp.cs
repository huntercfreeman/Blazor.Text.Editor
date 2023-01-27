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
        
        public const string TEXT_EDITOR_SERVICE_API = @"using System.Collections.Immutable;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib;

/// <summary>
/// <see cref=""TextEditorModelKey""/> is the unique identifier for the text editor that will be registered.<br/><br/>
/// (An example of one of the registration methods is <see cref=""RegisterTemplatedTextEditorModel""/>)<br/><br/>
/// The invoker of any registration method is highly likely to want to have a way to reference to the registered text editor.
/// <br/><br/> Therefore, the invoker must provide a <see cref=""TextEditorModelKey""/> so they can perform invocations of other
/// methods on <see cref=""ITextEditorService""/> using the <see cref=""TextEditorModelKey""/> as an identifying parameter.
/// </summary>
public interface ITextEditorService : IDisposable
{
    public const string LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY = ""bte_text-editor-options"";

    public TextEditorModelsCollection TextEditorModelsCollection { get; }
    public TextEditorGlobalOptions TextEditorGlobalOptions { get; }
    public ThemeRecord? GlobalThemeValue { get; }
    public string GlobalThemeCssClassString { get; }
    public string GlobalFontSizeInPixelsStyling { get; }
    public bool GlobalShowNewlines { get; }
    public bool GlobalShowWhitespace { get; }
    public int GlobalFontSizeInPixelsValue { get; }
    public double GlobalCursorWidthInPixelsValue { get; }
    public KeymapDefinition GlobalKeymapDefinition { get; }
    public int? GlobalHeightInPixelsValue { get; }

    public event Action? ModelsCollectionChanged;
    public event Action? ViewModelsCollectionChanged;
    public event Action? GroupsCollectionChanged;
    public event Action? GlobalOptionsChanged;

    /// <summary>
    /// It is recommended to use the <see cref=""RegisterTemplatedTextEditorModel""/> method
    /// as it will internally reference the <see cref=""ILexer""/> and
    /// <see cref=""IDecorationMapper""/> that correspond to the desired text editor.
    /// </summary>
    public void RegisterCustomTextEditorModel(TextEditorModel textEditorModel);
    /// <summary>
    /// As an example, for a C# Text Editor one would pass in a <see cref=""WellKnownModelKind""/>
    /// of <see cref=""WellKnownModelKind.CSharp""/>.
    /// <br/><br/>
    /// For a Plain Text Editor one would pass in a <see cref=""WellKnownModelKind""/>
    /// of <see cref=""WellKnownModelKind.Plain""/>.
    /// </summary>
    public void RegisterTemplatedTextEditorModel(
        TextEditorModelKey textEditorModelKey,
        WellKnownModelKind wellKnownModelKind,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent);
    
    public string? GetAllText(TextEditorModelKey textEditorModelKey);
    public string? GetAllText(TextEditorViewModelKey textEditorViewModelKey);
    public void InsertText(TextEditorModelsCollection.InsertTextAction insertTextAction);
    public void HandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction);
    public void DeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction);
    public void DeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction);
    public void RedoEdit(TextEditorModelKey textEditorModelKey);
    public void UndoEdit(TextEditorModelKey textEditorModelKey);
    public void DisposeTextEditor(TextEditorModelKey textEditorModelKey);
    public void SetFontSize(int fontSizeInPixels);
    public void SetCursorWidth(double cursorWidthInPixels);
    public void SetHeight(int? heightInPixels);
    /// <summary>
    /// This is setting the TextEditor's theme specifically.
    /// This is not to be confused with the ""BlazorALaCarte.Shared"" Themes which
    /// get applied at an application level.
    /// <br/><br/>
    /// This allows for a ""DarkTheme-Application"" that has a ""LightTheme-TextEditor""
    /// </summary>
    public void SetTheme(ThemeRecord theme);
    public void SetKeymap(KeymapDefinition foundKeymap);
    public void SetShowWhitespace(bool showWhitespace);
    public void SetShowNewlines(bool showNewlines);
    public void SetUsingRowEndingKind(
        TextEditorModelKey textEditorModelKey,
        RowEndingKind rowEndingKind);
    public void SetResourceData(
        TextEditorModelKey textEditorModelKey,
        string resourceUri,
        DateTime resourceLastWriteTime);
    
    public void ShowSettingsDialog(bool isResizable = false);

    public void RegisterGroup(TextEditorGroupKey textEditorGroupKey);
    public void AddViewModelToGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void RemoveViewModelFromGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void SetActiveViewModelOfGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    
    public void RegisterViewModel(TextEditorViewModelKey textEditorViewModelKey, TextEditorModelKey textEditorModelKey);
    public ImmutableArray<TextEditorViewModel> GetViewModelsForModel(TextEditorModelKey textEditorModelKey);    
    public TextEditorModel? GetTextEditorModelFromViewModelKey(TextEditorViewModelKey textEditorViewModelKey);
    public void SetViewModelWith(TextEditorViewModelKey textEditorViewModelKey, Func<TextEditorViewModel, TextEditorViewModel> withFunc);

    public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTop);
    
    public Task MutateScrollHorizontalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task MutateScrollVerticalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task SetScrollPositionAsync(string bodyElementId, string gutterElementId, double? scrollLeft, double? scrollTop);
    
    public TextEditorModel? GetTextEditorModelOrDefaultByResourceUri(string resourceUri);
    public void ReloadTextEditorModel(TextEditorModelKey textEditorModelKey, string content);
    
    public Task<ElementMeasurementsInPixels> GetElementMeasurementsInPixelsById(string elementId);

    public TextEditorModel? GetTextEditorModelOrDefault(TextEditorModelKey textEditorModelKey);
    public TextEditorViewModel? GetTextEditorViewModelOrDefault(TextEditorViewModelKey textEditorViewModelKey);
    public TextEditorGroup? GetTextEditorGroupOrDefault(TextEditorGroupKey textEditorGroupKey);
    
    public Task FocusPrimaryCursorAsync(string primaryCursorContentId);
    
    public Task SetTextEditorOptionsFromLocalStorageAsync();
    public void WriteGlobalTextEditorOptionsToLocalStorage();
}";
    }
}