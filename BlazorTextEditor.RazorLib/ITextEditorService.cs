using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib;

public interface ITextEditorService : IDisposable
{
    public TextEditorStates TextEditorStates { get; }
    
    public string GlobalThemeCssClassString { get; }
    public string GlobalFontSizeInPixelsStyling { get; }
    public bool GlobalShowNewlines { get; }
    public bool GlobalShowWhitespace  { get; }
    
    public event EventHandler? OnTextEditorStatesChanged;

    public void RegisterTextEditor(TextEditorBase textEditorBase);
    public void EditTextEditor(EditTextEditorBaseAction editTextEditorBaseAction);
    public void DisposeTextEditor(TextEditorKey textEditorKey);
    public void SetFontSize(int fontSizeInPixels);
    public void SetTheme(Theme theme);
    public void SetShowWhitespace(bool showWhitespace);
    public void SetShowNewlines(bool showNewlines);
    public void SetUsingRowEndingKind(TextEditorKey textEditorKey, RowEndingKind rowEndingKind);
}