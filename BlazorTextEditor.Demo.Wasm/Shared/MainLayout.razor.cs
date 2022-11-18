using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.Wasm.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.SetTextEditorOptionsFromLocalStorageAsync();

            await InvokeAsync(StateHasChanged);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
}