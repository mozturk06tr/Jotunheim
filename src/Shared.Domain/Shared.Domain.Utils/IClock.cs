namespace Shared.Domain.Utils;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
