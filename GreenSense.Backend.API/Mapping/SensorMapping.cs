using GreenSense.Backend.API.Dtos.Sensors;
using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Mapping;

public static class SensorMapping
{
    public static Sensor ToEntity(this SensorCreateRequest request)
    {
        return new Sensor
        {
            PlantId = request.PlantId,
            SensorType = request.SensorType,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyUpdate(this Sensor sensor, SensorUpdateRequest request)
    {
        sensor.IsActive = request.IsActive;
    }

    public static SensorResponse ToResponse(this Sensor sensor)
    {
        return new SensorResponse(
            sensor.SensorId,
            sensor.PlantId,
            sensor.SensorType,
            sensor.IsActive,
            sensor.CreatedAt
        );
    }
}
