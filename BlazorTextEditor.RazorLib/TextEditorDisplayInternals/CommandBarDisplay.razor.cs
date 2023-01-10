using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class CommandBarDisplay : FluxorComponent
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<Task> RestoreFocusToTextEditor { get; set; } = null!;

    private ElementReference? _commandBarDisplayElementReference;
    
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _commandBarDisplayElementReference?.FocusAsync();
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await RestoreFocusToTextEditor.Invoke();

            TextEditorService.SetViewModelWith(
                TextEditorViewModel.TextEditorViewModelKey,
                previousViewModel => previousViewModel with
                {
                    CommandBarValue = string.Empty,
                    DisplayCommandBar = false
                });
        }
    }
}