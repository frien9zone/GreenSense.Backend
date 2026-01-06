using GreenSense.Backend.API.Dtos.Readings;
using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Mapping;

public static class ReadingMapping
{
    public static SensorReading ToEntity(this ReadingCreateRequest request)
    {
        return new SensorReading
        {
            SensorId = request.SensorId,
            Value = request.Value,
            MeasuredAt = request.MeasuredAt ?? DateTime.UtcNow
        };
    }

    public static ReadingResponse ToResponse(this SensorReading reading)
    {
        return new ReadingResponse(
            reading.ReadingId,
            reading.SensorId,
            reading.Value,
            reading.MeasuredAt
        );
    }
}
