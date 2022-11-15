using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

/// <summary>
/// <see cref="TextEditorView"/> is the
/// message broker abstraction between a
/// Blazor component and a <see cref="TextEditorBase"/>
/// </summary>
public class TextEditorView : ComponentBase
{
    [Inject]
    protected IStateSelection<TextEditorStates, TextEditorBase?> TextEditorStatesSelection { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        // TODO: Will a IStateSelection only rerender if the selected state changes or any the parent FeatureState changes? This should only re-render when the TextEditorKey selected TextEditorBase is instantiated.
        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorList
                .SingleOrDefault(x => x.Key == TextEditorKey));

        base.OnInitialized();
    }
}