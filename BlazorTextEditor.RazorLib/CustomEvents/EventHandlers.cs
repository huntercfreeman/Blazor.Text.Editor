using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.CustomEvents;

/// <summary>
/// "onkeydownwithpreventscroll"
/// is used as a custom Blazor event.
/// <br/><br/>
/// The purpose is to conditionally
/// preventDefault on a @onkeydown event.
/// <br/><br/>
/// In specific this will preventDefault
/// on:
/// <br/>
/// -ContextMenu
/// -ArrowLeft
/// <br/>
/// -ArrowDown
/// <br/>
/// -ArrowUp
/// <br/>
/// -ArrowRight
/// <br/>
/// -Home
/// <br/>
/// -End
/// <br/>
/// -Space
/// <br/>
/// -Enter
/// </summary>
[EventHandler("onkeydownwithpreventscroll", typeof(KeyboardEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}