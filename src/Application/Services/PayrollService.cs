using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PayrollService : IPayrollService
{
    private readonly IPayrollRunRepository _payrollRepository;

    public PayrollService(IPayrollRunRepository payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PayrollRunDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var runs = await _payrollRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return runs.Select(run => run.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<PayrollRunDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var run = await _payrollRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return run?.ToDto();
    }

    /// <inheritdoc />
    public async Task<PayrollRunDto> CreateAsync(CreatePayrollRunRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _payrollRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<PayrollRunDto?> UpdateAsync(Guid id, UpdatePayrollRunRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _payrollRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _payrollRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _payrollRepository.RemoveAsync(id, cancellationToken);
    }
}
