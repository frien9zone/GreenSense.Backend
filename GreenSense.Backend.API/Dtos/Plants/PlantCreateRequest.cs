namespace GreenSense.Backend.API.Dtos.Plants;

public record PlantCreateRequest(
    int UserId,
    string Name,
    string? Description
);
