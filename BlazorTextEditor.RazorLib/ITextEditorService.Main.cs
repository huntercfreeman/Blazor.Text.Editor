using System.Collections.Immutable;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.Diff;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

/// <summary>Methods are prepended by the State which is most meaningfully involved when invoking the method.<br/><br/>Example, the method "ModelRedoEdit(...)" will most meaningfully involve the <see cref="TextEditorModel"/>. Therefore it starts with the word "Model".<br/><br/>The "TextEditor" part of "TextEditorModel" is left off because the method is scoped to the <see cref="ITextEditorService"/>. Therefore, we can assume that "Model" refers to "TextEditorModel" unless explicitly stated otherwise.</summary>
public partial interface ITextEditorService
{
    #region PropertiesNoSortingYet
    
    public IState<TextEditorModelsCollection> ModelsCollectionWrap { get; }
    public IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; }
    public IState<TextEditorGroupsCollection> GroupsCollectionWrap { get; }
    public IState<TextEditorDiffsCollection> DiffsCollectionWrap { get; }
    public IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; }
    public IState<TextEditorOptionsState> OptionsWrap { get; }
    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    #endregion
    
    #region MethodsSortedAlphabetically
    
    public Task CursorPrimaryFocusAsync(string primaryCursorContentId);
    public TextEditorDiffResult? DiffCalculate(TextEditorDiffKey textEditorDiffKey, CancellationToken cancellationToken);
    public void DiffDispose(TextEditorDiffKey textEditorDiffKey);
    public TextEditorDiffModel? DiffModelFindOrDefault(TextEditorDiffKey textEditorDiffKey);
    public void DiffRegister(TextEditorDiffKey diffKey, TextEditorViewModelKey beforeViewModelKey, TextEditorViewModelKey afterViewModelKey);
    public Task<ElementMeasurementsInPixels> ElementMeasurementsInPixelsAsync(string elementId);
    public void OptionsSetCursorWidth(double cursorWidthInPixels);
    public void OptionsSetFontFamily(string? fontFamily);
    public void OptionsSetFontSize(int fontSizeInPixels);
    public Task OptionsSetFromLocalStorageAsync();
    public void OptionsSetHeight(int? heightInPixels);
    public void OptionsSetKeymap(KeymapDefinition foundKeymap);
    public void OptionsSetShowNewlines(bool showNewlines);
    public void OptionsSetUseMonospaceOptimizations(bool useMonospaceOptimizations);
    public void OptionsSetShowWhitespace(bool showWhitespace);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
    public void OptionsSetTheme(ThemeRecord theme);
    public void OptionsShowSettingsDialog(bool isResizable = false, string? cssClassString = null);
    public void OptionsWriteToStorage();
    
    #endregion

    public IModelApi Model { get; }
    public IViewModelApi ViewModel { get; }
    public IGroupApi Group { get; }
}