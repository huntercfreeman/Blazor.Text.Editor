using System.Collections.Immutable;
using BlazorALaCarte.Shared.Facts;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;

[FeatureState]
public partial class TextEditorModelsCollection
{
    public ImmutableList<TextEditorModel> TextEditorList { get; init; }
    public TextEditorOptions GlobalTextEditorOptions { get; init; }
    
    public TextEditorModelsCollection()
    {
        TextEditorList = ImmutableList<TextEditorModel>.Empty;
        
        GlobalTextEditorOptions = new TextEditorOptions(
            20,
            ThemeFacts.VisualStudioDarkThemeClone,
            false,
            false,
            null,
            2.5,
            KeymapFacts.DefaultKeymapDefinition);
    }
}