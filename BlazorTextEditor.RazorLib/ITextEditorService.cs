using System.Collections.Immutable;
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
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib;

/// <summary>
/// <see cref="TextEditorKey"/> is the unique identifier for the text editor that will be registered.<br/><br/>
/// (An example of one of the registration methods is <see cref="RegisterCSharpTextEditor"/>)<br/><br/>
/// The invoker of any registration method is highly likely to want to have a way to reference to the registered text editor.
/// <br/><br/> Therefore, the invoker must provide a <see cref="TextEditorKey"/> so they can perform invocations of other
/// methods on <see cref="ITextEditorService"/> using the <see cref="TextEditorKey"/> as an identifying parameter.
/// </summary>
public interface ITextEditorService : IDisposable
{
    public const string LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY = "bte_text-editor-options";

    public TextEditorStates TextEditorStates { get; }
    public ThemeRecord? GlobalThemeValue { get; }
    public string GlobalThemeCssClassString { get; }
    public string GlobalFontSizeInPixelsStyling { get; }
    public bool GlobalShowNewlines { get; }
    public bool GlobalShowWhitespace { get; }
    public int GlobalFontSizeInPixelsValue { get; }
    public double GlobalCursorWidthInPixelsValue { get; }
    public KeymapDefinition GlobalKeymapDefinition { get; }
    public int? GlobalHeightInPixelsValue { get; }

    public event Action? OnTextEditorStatesChanged;

    /// <summary>
    /// It is recommended to use the other Register methods as they will internally reference the <see cref="ILexer"/> and
    /// <see cref="IDecorationMapper"/> that correspond to the desired text editor.<br/><br/>
    /// For example: invoke <see cref="RegisterCSharpTextEditor"/> to register a TextEditorBase for use with C# source code.
    /// </summary>
    public void RegisterCustomTextEditor(TextEditorBase textEditorBase);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorCSharpLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorCSharpDecorationMapper"/>
    /// </summary>
    public void RegisterCSharpTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorHtmlLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorHtmlDecorationMapper"/>
    /// </summary>
    public void RegisterHtmlTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorCssLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorCssDecorationMapper"/>
    /// </summary>
    public void RegisterCssTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorJsonLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorJsonDecorationMapper"/>
    /// </summary>
    public void RegisterJsonTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorFSharpLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorFSharpDecorationMapper"/>
    /// </summary>
    public void RegisterFSharpTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorRazorLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorHtmlDecorationMapper"/>
    /// </summary>
    public void RegisterRazorTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorJavaScriptLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorJavaScriptDecorationMapper"/>
    /// </summary>
    public void RegisterJavaScriptTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorTypeScriptLexer"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorTypeScriptDecorationMapper"/>
    /// </summary>
    public void RegisterTypeScriptTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    /// <summary>
    /// Constructs a new <see cref="TextEditorBase"/> using the <see cref="TextEditorKey"/> provided. The text editor will
    /// render with the <see cref="initialContent"/> provided. <see cref="ITextEditorKeymap"/> is optional and it is likely
    /// that one would prefer leaving it null as this will result in the default keymap being used.<br/><br/>
    /// Used <see cref="ILexer"/>: <see cref="TextEditorLexerDefault"/><br/>
    /// Used <see cref="IDecorationMapper"/>: <see cref="TextEditorDecorationMapperDefault"/>
    /// </summary>
    public void RegisterPlainTextEditor(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string initialContent,
        ITextEditorKeymap? textEditorKeymapOverride = null);
    public void InsertText(InsertTextTextEditorBaseAction insertTextTextEditorBaseAction);
    public void HandleKeyboardEvent(KeyboardEventTextEditorBaseAction keyboardEventTextEditorBaseAction);
    public void DeleteTextByMotion(DeleteTextByMotionTextEditorBaseAction deleteTextByMotionTextEditorBaseAction);
    public void DeleteTextByRange(DeleteTextByRangeTextEditorBaseAction deleteTextByRangeTextEditorBaseAction);
    public void RedoEdit(TextEditorKey textEditorKey);
    public void UndoEdit(TextEditorKey textEditorKey);
    public void DisposeTextEditor(TextEditorKey textEditorKey);
    public void SetFontSize(int fontSizeInPixels);
    public void SetCursorWidth(double cursorWidthInPixels);
    public void SetHeight(int? heightInPixels);
    public void SetTheme(ThemeRecord theme);
    public void SetKeymap(KeymapDefinition foundKeymap);
    public void SetShowWhitespace(bool showWhitespace);
    public void SetShowNewlines(bool showNewlines);
    public void SetUsingRowEndingKind(
        TextEditorKey textEditorKey,
        RowEndingKind rowEndingKind);
    public void SetResourceData(
        TextEditorKey textEditorKey,
        string resourceUri,
        DateTime resourceLastWriteTime);
    
    public void ShowSettingsDialog(bool isResizable = false);

    public void RegisterGroup(TextEditorGroupKey textEditorGroupKey);
    public void AddViewModelToGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void RemoveViewModelFromGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    public void SetActiveViewModelOfGroup(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    
    public void RegisterViewModel(TextEditorViewModelKey textEditorViewModelKey, TextEditorKey textEditorKey);
    public ImmutableArray<TextEditorViewModel> GetViewModelsForTextEditorBase(TextEditorKey textEditorKey);    
    public TextEditorBase? GetTextEditorBaseFromViewModelKey(TextEditorViewModelKey textEditorViewModelKey);
    public void SetViewModelWith(TextEditorViewModelKey textEditorViewModelKey, Func<TextEditorViewModel, TextEditorViewModel> withFunc);

    public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTop);
    
    public Task MutateScrollHorizontalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task MutateScrollVerticalPositionByPixelsAsync(string bodyElementId, string gutterElementId, double pixels);
    public Task SetScrollPositionAsync(string bodyElementId, string gutterElementId, double? scrollLeft, double? scrollTop);
    
    public TextEditorBase? GetTextEditorBaseOrDefaultByResourceUri(string resourceUri);
    public void ReloadTextEditorBase(TextEditorKey textEditorKey, string content);
    
    public Task<ElementMeasurementsInPixels> GetElementMeasurementsInPixelsById(string elementId);
    
    public Task FocusPrimaryCursorAsync(string primaryCursorContentId);
    
    public Task SetTextEditorOptionsFromLocalStorageAsync();
    public void WriteGlobalTextEditorOptionsToLocalStorage();
}