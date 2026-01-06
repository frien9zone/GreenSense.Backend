using System.Threading;
using System.Threading.Tasks;

namespace GreenSense.Backend.API.Services;

public interface IReadingNotificationService
{
    Task EvaluateAndCreateNotificationIfNeededAsync(int readingId, CancellationToken ct = default);
}
