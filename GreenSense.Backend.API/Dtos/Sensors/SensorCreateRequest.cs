using GreenSense.Backend.Data.Enums;

namespace GreenSense.Backend.API.Dtos.Sensors;

public record SensorCreateRequest(
    int PlantId,
    SensorType SensorType,
    bool IsActive
);
