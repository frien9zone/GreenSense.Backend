namespace GreenSense.Backend.API.Dtos.Readings;

public record ReadingResponse(
    int ReadingId,
    int SensorId,
    double Value,
    DateTime MeasuredAt
);
