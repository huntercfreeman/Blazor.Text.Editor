using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Notifications;

public partial class CommonInformativeNotificationDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
}