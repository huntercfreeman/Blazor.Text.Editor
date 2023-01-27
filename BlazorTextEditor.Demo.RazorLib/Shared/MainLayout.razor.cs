using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.GlobalOptionsChanged += TextEditorServiceOnGlobalOptionsChanged;
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.GlobalOptionsSetFromLocalStorageAsync();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void TextEditorServiceOnGlobalOptionsChanged()
    {
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        TextEditorService.GlobalOptionsChanged -= TextEditorServiceOnGlobalOptionsChanged;
    }
}