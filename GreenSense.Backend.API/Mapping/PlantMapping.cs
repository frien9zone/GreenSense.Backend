using GreenSense.Backend.API.Dtos.Plants;
using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Mapping;

public static class PlantMapping
{
    public static Plant ToEntity(this PlantCreateRequest request)
    {
        return new Plant
        {
            UserId = request.UserId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyUpdate(this Plant plant, PlantUpdateRequest request)
    {
        plant.Name = request.Name;
        plant.Description = request.Description;
    }

    public static PlantResponse ToResponse(this Plant plant)
    {
        return new PlantResponse(
            plant.PlantId,
            plant.UserId,
            plant.Name,
            plant.Description,
            plant.CreatedAt
        );
    }
}
