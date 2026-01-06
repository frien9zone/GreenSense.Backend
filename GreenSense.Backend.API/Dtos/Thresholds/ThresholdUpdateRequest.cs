namespace GreenSense.Backend.API.Dtos.Thresholds;

public record ThresholdUpdateRequest(
    double SoilMin,
    double SoilMax,
    double TempMin,
    double TempMax
);
