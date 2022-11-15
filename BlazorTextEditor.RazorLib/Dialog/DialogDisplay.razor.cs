using BlazorTextEditor.RazorLib.Resize;
using BlazorTextEditor.RazorLib.Store.DialogCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Dialog;

public partial class DialogDisplay : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private ResizableDisplay? _resizableDisplay;

    private string StyleCssString => DialogRecord.ElementDimensions.StyleString;
    
    private string GlobalThemeCssClassString => TextEditorService
                                                    .TextEditorStates
                                                    .GlobalTextEditorOptions
                                                    .Theme?
                                                    .CssClassString
                                                ?? string.Empty;
    
    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void SubscribeMoveHandle()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
    }
    
    private void DispatchDisposeDialogRecordAction()
    {
        Dispatcher.Dispatch(new DisposeDialogRecordAction(DialogRecord));
    }
}