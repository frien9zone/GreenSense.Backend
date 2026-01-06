namespace GreenSense.Backend.API.Dtos.Users;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
