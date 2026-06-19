namespace Damoor.Application.Common.Exceptions;

public sealed class BadRequestException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public BadRequestException(
        string message,
        IEnumerable<string>? errors = null)
        : base(message)
    {
        Errors = errors?.ToArray() ?? [];
    }
}
