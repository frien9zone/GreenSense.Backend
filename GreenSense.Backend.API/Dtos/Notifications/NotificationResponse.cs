namespace GreenSense.Backend.API.Dtos.Notifications;

public record NotificationResponse(
    int NotificationId,
    int PlantId,
    int? ReadingId,
    string Type,
    string Message,
    DateTime CreatedAt,
    bool IsRead
);
