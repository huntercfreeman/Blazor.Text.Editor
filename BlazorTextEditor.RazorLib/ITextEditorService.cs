using System.Collections.Immutable;
using BlazorALaCarte.Shared.Storage;
using BlazorALaCarte.Shared.Store.ThemeCase;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.GlobalOptions;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

/// <summary>Methods are prepended by the State which is most meaningfully involved when invoking the method.<br/><br/>Example, the method "ModelRedoEdit(...)" will most meaningfully involve the <see cref="TextEditorModel"/>. Therefore it starts with the word "Model".<br/><br/>The "TextEditor" part of "TextEditorModel" is left off because the method is scoped to the <see cref="ITextEditorService"/>. Therefore, we can assume that "Model" refers to "TextEditorModel" unless explicitly stated otherwise.</summary>
public interface ITextEditorService
{
    #region PropertiesNoSortingYet
    
    public IState<TextEditorModelsCollection> ModelsCollectionWrap { get; }
    public IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; }
    public IState<TextEditorGroupsCollection> GroupsCollectionWrap { get; }
    public IState<TextEditorDiffsCollection> DiffsCollectionWrap { get; }
    public IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; }
    public IState<TextEditorGlobalOptions> GlobalOptionsWrap { get; }
    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string GlobalThemeCssClassString { get; }

    #endregion
    
    #region MethodsSortedAlphabetically
    
    public Task CursorPrimaryFocusAsync(string primaryCursorContentId);
    public TextEditorDiffResult? DiffCalculate(TextEditorDiffKey textEditorDiffKey, CancellationToken cancellationToken);
    public void DiffDispose(TextEditorDiffKey textEditorDiffKey);
    public TextEditorDiffModel? DiffModelFindOrDefault(TextEditorDiffKey textEditorDiffKey);
    public void DiffRegister(TextEditorDiffKey diffKey, TextEditorViewModelKey beforeViewModelKey, TextEditorViewModelKey afterViewModelKey);
    public Task<ElementMeasurementsInPixels> ElementMeasurementsInPixelsAsync(string elementId);
    public void GlobalOptionsSetCursorWidth(double cursorWidthInPixels);
    public void GlobalOptionsSetFontSize(int fontSizeInPixels);
    public Task GlobalOptionsSetFromLocalStorageAsync();
    public void GlobalOptionsSetHeight(int? heightInPixels);
    public void GlobalOptionsSetKeymap(KeymapDefinition foundKeymap);
    public void GlobalOptionsSetShowNewlines(bool showNewlines);
    public void GlobalOptionsSetShowWhitespace(bool showWhitespace);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the "BlazorALaCarte.Shared" Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
    public void GlobalOptionsSetTheme(ThemeRecord theme);
    public void GlobalOptionsShowSettingsDialog(bool isResizable = false);
    public void GlobalOptionsWriteToStorage();
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
    /// <summary>It is recommended to use the <see cref="ModelRegisterTemplatedModel" /> method as it will internally reference the <see cref="ILexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
    public void ModelRegisterCustomModel(TextEditorModel model);
    /// <summary>As an example, for a C# Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.CSharp" />.<br /><br />For a Plain Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.Plain" />.</summary>
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
}