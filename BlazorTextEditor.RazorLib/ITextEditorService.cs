﻿using System.Collections.Immutable;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Diff;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib;

/// <summary>Methods are prepended by the State which is most meaningfully involved when invoking the method.<br/><br/>Example, the method "ModelRedoEdit(...)" will most meaningfully involve the <see cref="TextEditorModel"/>. Therefore it starts with the word "Model".<br/><br/>The "TextEditor" part of "TextEditorModel" is left off because the method is scoped to the <see cref="ITextEditorService"/>. Therefore, we can assume that "Model" refers to "TextEditorModel" unless explicitly stated otherwise.</summary>
public interface ITextEditorService : IDisposable
{
    #region ConstantsSortedAlphabetically

    /// <summary>Used as the key storing the <see cref="GlobalOptions"/> using JavaScript and local storage.</summary>
    public const string LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY = "bte_text-editor-options";

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
    /// <summary>Contains all registered <see cref="TextEditorDiff"/></summary>
    public TextEditorDiffsCollection DiffsCollection { get; }
    public TextEditorGlobalOptions GlobalOptions { get; }
    /// <summary>Contains all registered <see cref="TextEditorGroup"/></summary>
    public TextEditorGroupsCollection GroupsCollection { get; }
    /// <summary>Contains all registered <see cref="TextEditorModel"/></summary>
    public TextEditorModelsCollection ModelsCollection { get; }
    /// <summary>Contains all registered <see cref="TextEditorViewModel"/></summary>
    public TextEditorViewModelsCollection ViewModelsCollection { get; }

    #endregion

    #region EventsSortedAlphabetically

    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref="TextEditorDiff"/><br/>-On dispose of a <see cref="TextEditorDiff"/><br/>-On replacement of an immutable <see cref="TextEditorDiff"/> which is contained within the <see cref="TextEditorDiffsCollection"/></summary>
    public event Action? DiffsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On replacement of the immutable <see cref="TextEditorOptions"/> which is contained within <see cref="TextEditorGlobalOptions"/></summary>
    public event Action? GlobalOptionsChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref="TextEditorGroup"/><br/>-On dispose of a <see cref="TextEditorGroup"/><br/>-On replacement of an immutable <see cref="TextEditorGroup"/> which is contained within the <see cref="TextEditorGroupsCollection"/></summary>
    public event Action? GroupsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref="TextEditorModel"/><br/>-On dispose of a <see cref="TextEditorModel"/><br/>-On replacement of an immutable <see cref="TextEditorModel"/> which is contained within the <see cref="TextEditorModelsCollection"/></summary>
    public event Action? ModelsCollectionChanged;
    /// <summary>This event is known to fire in the following conditions.<br/>-On registration of a <see cref="TextEditorViewModel"/><br/>-On dispose of a <see cref="TextEditorViewModel"/><br/>-On replacement of an immutable <see cref="TextEditorViewModel"/> which is contained within the <see cref="TextEditorViewModelsCollection"/></summary>
    public event Action? ViewModelsCollectionChanged;

    #endregion

    #region MethodsSortedAlphabetically
    
    public void AddViewModelToGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void DeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction);
    public void DeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction);
    public void DisposeDiff(TextEditorDiffKey textEditorDiffKey);
    public void DisposeTextEditor(TextEditorModelKey textEditorModelKey);
    public Task FocusPrimaryCursorAsync(string primaryCursorContentId);
    public string? GetAllText(TextEditorModelKey textEditorModelKey);
    public string? GetAllText(TextEditorViewModelKey textEditorViewModelKey);
    public Task<ElementMeasurementsInPixels> GetElementMeasurementsInPixelsById(string elementId);
    public TextEditorGroup? GetTextEditorGroupOrDefault(TextEditorGroupKey textEditorGroupKey);
    public TextEditorModel? GetTextEditorModelFromViewModelKey(TextEditorViewModelKey textEditorViewModelKey);
    public TextEditorModel? GetTextEditorModelOrDefault(TextEditorModelKey textEditorModelKey);
    public TextEditorModel? GetTextEditorModelOrDefaultByResourceUri(string resourceUri);
    public TextEditorViewModel? GetTextEditorViewModelOrDefault(TextEditorViewModelKey textEditorViewModelKey);
    public ImmutableArray<TextEditorViewModel> GetViewModelsForModel(TextEditorModelKey textEditorModelKey);
    public void HandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction);
    public void InsertText(TextEditorModelsCollection.InsertTextAction insertTextAction);
    public Task MutateScrollHorizontalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task MutateScrollVerticalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public void RedoEdit(TextEditorModelKey textEditorModelKey);
    /// <summary>It is recommended to use the <see cref="RegisterTemplatedTextEditorModel" /> method as it will internally reference the <see cref="ILexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
    public void RegisterCustomTextEditorModel(TextEditorModel textEditorModel);
    public void RegisterDiff(TextEditorDiffKey diffKey, TextEditorViewModelKey beforeViewModelKey, TextEditorViewModelKey afterViewModelKey);
    public void RegisterGroup(TextEditorGroupKey textEditorGroupKey);
    /// <summary>As an example, for a C# Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.CSharp" />.<br /><br />For a Plain Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.Plain" />.</summary>
    public void RegisterTemplatedTextEditorModel(TextEditorModelKey textEditorModelKey, WellKnownModelKind wellKnownModelKind, string resourceUri, DateTime resourceLastWriteTime, string fileExtension, string initialContent);
    public void RegisterViewModel(TextEditorViewModelKey textEditorViewModelKey, TextEditorModelKey textEditorModelKey);
    public void ReloadTextEditorModel(TextEditorModelKey textEditorModelKey, string content);
    public void RemoveViewModelFromGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void SetActiveViewModelOfGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void SetCursorWidth(double cursorWidthInPixels);
    public void SetFontSize(int fontSizeInPixels);
    public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTop);
    public void SetHeight(int? heightInPixels);
    public void SetKeymap(KeymapDefinition foundKeymap);
    public void SetResourceData(TextEditorModelKey textEditorModelKey, string resourceUri, DateTime resourceLastWriteTime);
    public Task SetScrollPositionAsync(string bodyElementId, string gutterElementId, double? scrollLeft, double? scrollTop);
    public void SetShowNewlines(bool showNewlines);
    public void SetShowWhitespace(bool showWhitespace);
    public Task SetTextEditorOptionsFromLocalStorageAsync();
    public void SetTheme(ThemeRecord theme);
    public void SetUsingRowEndingKind(TextEditorModelKey textEditorModelKey, RowEndingKind rowEndingKind);
    public void SetViewModelWith(TextEditorViewModelKey textEditorViewModelKey, Func<TextEditorViewModel, TextEditorViewModel> withFunc);
    public void ShowSettingsDialog(bool isResizable = false);
    public void UndoEdit(TextEditorModelKey textEditorModelKey);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the "BlazorALaCarte.Shared" Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
    public void WriteGlobalTextEditorOptionsToLocalStorage();

    #endregion
}