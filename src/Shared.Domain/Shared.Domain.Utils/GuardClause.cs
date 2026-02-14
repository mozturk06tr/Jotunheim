namespace Shared.Domain.Utils;

// <summary>
// A utility class for validating method parameters and state conditions.
// </summary>
internal static class Guard
{
    internal static void GuidNotEmpty(Guid value, string paramName)
        => _ = value != Guid.Empty
            ? value
            : throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    internal static void NotZero(decimal value, string paramName)
        => _ = value != 0m
            ? value
            : throw new ArgumentException($"{paramName} cannot be zero.", paramName);
    internal static void NotBefore(DateTimeOffset value, DateTimeOffset threshold, string paramName)
        => _ = value >= threshold
            ? value
            : throw new ArgumentException($"{paramName} cannot be before {threshold}.", paramName);
    internal static void False(bool condition, string message)
        => _ = !condition
            ? true
            : throw new InvalidOperationException(message);
    internal static void NotFuture(DateTimeOffset value, DateTimeOffset threshold, string message)
        => _ = value <= threshold
            ? value
            : throw new ArgumentException(message);
}
