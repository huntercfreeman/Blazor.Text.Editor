using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Model;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.ViewModel.InternalComponents;

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
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (_commandBarDisplayElementReference is not null)
                    await _commandBarDisplayElementReference.Value.FocusAsync();
            }
            catch (Exception e)
            {
                // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                //             This bug is seemingly happening randomly. I have a suspicion
                //             that there are race-condition exceptions occurring with "FocusAsync"
                //             on an ElementReference.
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await RestoreFocusToTextEditor.Invoke();

            TextEditorService.ViewModel.With(
                TextEditorViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    CommandBarValue = string.Empty,
                    DisplayCommandBar = false
                });
        }
    }
}