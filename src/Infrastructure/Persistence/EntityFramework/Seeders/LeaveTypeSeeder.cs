using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Infrastructure.Persistence.EntityFramework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Seeders;

public static class LeaveTypeSeeder
{
    private static readonly IReadOnlyCollection<LeaveType> DefaultLeaveTypes = new[]
    {
        new LeaveType
        {
            Id = Guid.Parse("63c4f05d-d7d4-4f95-8025-1dfc3b3f3f2a"),
            Code = "VACATION",
            Name = "Vacation",
            IsPaid = true,
            RequiresApproval = true,
            RequiresAttachment = false,
            AnnualAllowanceDays = 20m,
            CarryOverDays = 5m,
            MaxConsecutiveDays = 10,
            Color = "#4CAF50"
        },
        new LeaveType
        {
            Id = Guid.Parse("a4a4bc58-0c49-4b5f-bd4a-15ccf0ae5414"),
            Code = "SICK",
            Name = "Sick",
            IsPaid = true,
            RequiresApproval = false,
            RequiresAttachment = false,
            AnnualAllowanceDays = 10m,
            CarryOverDays = 2m,
            MaxConsecutiveDays = 5,
            Color = "#F44336"
        },
        new LeaveType
        {
            Id = Guid.Parse("f41074cf-3cd5-4b52-92ea-4c1d77c7d4d1"),
            Code = "PERSONAL",
            Name = "Personal",
            IsPaid = true,
            RequiresApproval = true,
            RequiresAttachment = false,
            AnnualAllowanceDays = 5m,
            CarryOverDays = 0m,
            MaxConsecutiveDays = 3,
            Color = "#FF9800"
        }
    };

    public static async Task EnsureSeededAsync(HrDbContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var existingCodes = await context.LeaveTypes
            .Select(type => type.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var missing = DefaultLeaveTypes
            .Where(type => !existingCodes.Contains(type.Code, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        if (missing.Length == 0)
        {
            return;
        }

        await context.LeaveTypes.AddRangeAsync(missing, cancellationToken).ConfigureAwait(false);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
