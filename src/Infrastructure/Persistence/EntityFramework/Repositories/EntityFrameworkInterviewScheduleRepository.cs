using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkInterviewScheduleRepository
    : EntityFrameworkRepository<InterviewSchedule>, IInterviewScheduleRepository
{
    public EntityFrameworkInterviewScheduleRepository(HrDbContext dbContext)
        : base(dbContext, schedule => schedule.Id)
    {
    }

    public async Task<IReadOnlyCollection<InterviewSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var interviews = await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
        return interviews
            .OrderBy(interview => interview.ScheduledAtUtc)
            .ToArray();
    }

    public Task<InterviewSchedule?> GetByIdAsync(Guid interviewId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(interviewId, cancellationToken);
    }

    public Task<InterviewSchedule> AddAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(schedule, cancellationToken);
    }

    public Task<InterviewSchedule?> UpdateAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(schedule, cancellationToken);
    }
}
