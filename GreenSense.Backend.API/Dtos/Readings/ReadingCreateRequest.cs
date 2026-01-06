namespace GreenSense.Backend.API.Dtos.Readings;

public record ReadingCreateRequest(
    int SensorId,
    double Value,
    DateTime? MeasuredAt
);
