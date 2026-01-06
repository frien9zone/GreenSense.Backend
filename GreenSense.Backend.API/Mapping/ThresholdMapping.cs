using GreenSense.Backend.API.Dtos.Thresholds;
using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Mapping;

public static class ThresholdMapping
{
    public static void ApplyUpdate(this ThresholdSettings entity, ThresholdUpdateRequest request)
    {
        entity.SoilMin = request.SoilMin;
        entity.SoilMax = request.SoilMax;
        entity.TempMin = request.TempMin;
        entity.TempMax = request.TempMax;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static ThresholdResponse ToResponse(this ThresholdSettings entity)
    {
        return new ThresholdResponse(
            entity.ThresholdId,
            entity.PlantId,
            entity.SoilMin,
            entity.SoilMax,
            entity.TempMin,
            entity.TempMax,
            entity.UpdatedAt
        );
    }
}
