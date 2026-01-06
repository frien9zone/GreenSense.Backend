namespace GreenSense.Backend.API.Dtos.Plants;

public record PlantResponse(
    int PlantId,
    int UserId,
    string Name,
    string? Description,
    DateTime CreatedAt
);
