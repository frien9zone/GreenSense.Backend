using GreenSense.Backend.API.Dtos.Notifications;
using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Mapping;

public static class NotificationMapping
{
    public static NotificationResponse ToResponse(this Notification n)
    {
        return new NotificationResponse(
            n.NotificationId,
            n.PlantId,
            n.ReadingId,
            n.Type,
            n.Message,
            n.CreatedAt,
            n.IsRead
        );
    }
}
