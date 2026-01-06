using GreenSense.Backend.Data.Enums;

namespace GreenSense.Backend.API.Dtos.Sensors;

public record SensorResponse(
    int SensorId,
    int PlantId,
    SensorType SensorType,
    bool IsActive,
    DateTime CreatedAt
);
