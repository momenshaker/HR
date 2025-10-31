namespace HR.Application.Abstractions.Services;

public interface ILeaveRolloverService
{
    Task RunAsync(int newYear, CancellationToken cancellationToken = default);
}

