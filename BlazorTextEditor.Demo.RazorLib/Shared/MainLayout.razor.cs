using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.TextEditorModelsCollectionChanged += TextEditorServiceOnTextEditorModelsCollectionChanged;
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.SetTextEditorOptionsFromLocalStorageAsync();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void TextEditorServiceOnTextEditorModelsCollectionChanged()
    {
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        TextEditorService.TextEditorModelsCollectionChanged -= TextEditorServiceOnTextEditorModelsCollectionChanged;
    }
}