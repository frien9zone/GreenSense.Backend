namespace GreenSense.Backend.API.Dtos.Plants;

public record PlantUpdateRequest(
    string Name,
    string? Description
);
