namespace GreenSense.Backend.API.Dtos.Thresholds;

public record ThresholdResponse(
    int ThresholdId,
    int PlantId,
    double SoilMin,
    double SoilMax,
    double TempMin,
    double TempMax,
    DateTime UpdatedAt
);
