using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.DropdownCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Menu;

public partial class MenuOptionDisplay : ComponentBase
{
    [Inject]
    private IState<DropdownStates> DropdownStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Index { get; set; }
    [Parameter, EditorRequired]
    public int ActiveMenuOptionRecordIndex { get; set; }
    
    private readonly DropdownKey _subMenuDropdownKey = DropdownKey.NewDropdownKey();
    private ElementReference? _menuOptionDisplayElementReference;
    private bool _shouldDisplayWidget;

    private bool IsActive => Index == ActiveMenuOptionRecordIndex;
    private bool HasSubmenuActive => DropdownStatesWrap.Value.ActiveDropdownKeys
        .Any(x => x.Guid == _subMenuDropdownKey.Guid);
    
    private string IsActiveCssClass => IsActive
        ? "bstudio_active"
        : string.Empty;
    
    private string HasSubmenuActiveCssClass => HasSubmenuActive
            ? "bstudio_active"
            : string.Empty;
    
    private string HasWidgetActiveCssClass => _shouldDisplayWidget && 
                                              MenuOptionRecord.WidgetRendererType is not null
        ? "bstudio_active"
        : string.Empty;

    private MenuOptionWidgetParameters MenuOptionWidgetParameters => new(
        () => HideWidgetAsync(null), 
        HideWidgetAsync);

    private bool DisplayWidget => _shouldDisplayWidget &&
                                  MenuOptionRecord.WidgetRendererType is not null;

    protected override async Task OnParametersSetAsync()
    {
        var localHasSubmenuActive = HasSubmenuActive;

        // The following if(s) are not working. They result
        // in one never being able to open a submenu
        //
        // // Close submenu for non active menu option
        // if (!IsActive && localHasSubmenuActive)
        //     Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(_subMenuDropdownKey));
        //
        // // Hide widget for non active menu option
        // if (!IsActive && _shouldDisplayWidget)
        // {
        //     _shouldDisplayWidget = false;
        // }
        
        // Set focus to active menu option
        if (IsActive && 
            !localHasSubmenuActive &&
            !DisplayWidget &&
            _menuOptionDisplayElementReference.HasValue)
        {
            await _menuOptionDisplayElementReference.Value.FocusAsync();
        }
        
        await base.OnParametersSetAsync();
    }

    private void HandleOnClick()
    {
        if (MenuOptionRecord.OnClick is not null)
        {
            MenuOptionRecord.OnClick.Invoke();
            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
        }

        if (MenuOptionRecord.SubMenu is not null)
        {
            if (HasSubmenuActive)
                Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(_subMenuDropdownKey));
            else
                Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_subMenuDropdownKey));
        }

        if (MenuOptionRecord.WidgetRendererType is not null)
        {
            _shouldDisplayWidget = !_shouldDisplayWidget;
        }
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
                if (MenuOptionRecord.SubMenu is not null)
                    Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_subMenuDropdownKey));
                break;
        }
        
        switch (keyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                HandleOnClick();
                break;
        }
    }

    private async Task HideWidgetAsync(Action? onAfterWidgetHidden)
    {
        _shouldDisplayWidget = false;
        await InvokeAsync(StateHasChanged);

        if (onAfterWidgetHidden is null)
        {
            if (_menuOptionDisplayElementReference.HasValue)
                await _menuOptionDisplayElementReference.Value.FocusAsync();
        }
        else
        {
            onAfterWidgetHidden.Invoke();
            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
        }
    }
}