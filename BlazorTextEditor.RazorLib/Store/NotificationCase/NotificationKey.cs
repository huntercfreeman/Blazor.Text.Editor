namespace BlazorTextEditor.RazorLib.Store.NotificationCase;

public record NotificationKey(Guid Guid)
{
    public static NotificationKey NewNotificationKey()
    {
        return new(Guid.NewGuid());
    }
}