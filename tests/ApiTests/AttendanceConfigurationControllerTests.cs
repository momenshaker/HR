using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HR.Application.DTOs;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class AttendanceConfigurationControllerTests : IDisposable
{
    private readonly AuthenticatedApiFactory _factory = new();
    private readonly string _accessToken = JwtTestHelper.CreateToken(
        AuthenticatedApiFactory.Issuer,
        AuthenticatedApiFactory.Audience,
        AuthenticatedApiFactory.SigningKey,
        new[] { "Admin", "HR" });

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task WorkSchedulesController_CrudFlow_PersistsShiftTemplates()
    {
        using var client = CreateClient();
        var createRequest = new CreateWorkScheduleRequest
        {
            Name = "Standard 9-6",
            OrganizationId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            IsDefaultForOrganization = true,
            TimeZoneId = "UTC",
            ShiftTemplates = new[]
            {
                new ShiftTemplateRequest
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    BreakMinutes = 60,
                    GracePeriodMinutes = 10,
                    MinimumOvertimeMinutes = 30
                },
                new ShiftTemplateRequest
                {
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    BreakMinutes = 45,
                    GracePeriodMinutes = 5,
                    MinimumOvertimeMinutes = 60
                }
            }
        };

        var createResponse = await client
            .PostAsJsonAsync("/api/v1/WorkSchedules", createRequest)
            .ConfigureAwait(false);

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<WorkScheduleDto>().ConfigureAwait(false);
        Assert.NotNull(created);
        Assert.Equal(createRequest.Name, created!.Name);
        Assert.Equal(2, created.ShiftTemplates.Count);

        var list = await client
            .GetFromJsonAsync<WorkScheduleDto[]>("/api/v1/WorkSchedules")
            .ConfigureAwait(false);

        Assert.Contains(list!, schedule => schedule.Id == created.Id);

        var fetched = await client
            .GetFromJsonAsync<WorkScheduleDto>($"/api/v1/WorkSchedules/{created.Id}")
            .ConfigureAwait(false);

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);

        var updateRequest = new UpdateWorkScheduleRequest
        {
            Name = "Summer Hours",
            OrganizationId = createRequest.OrganizationId,
            DepartmentId = createRequest.DepartmentId,
            IsDefaultForOrganization = false,
            TimeZoneId = "Europe/Paris",
            ShiftTemplates = new[]
            {
                new ShiftTemplateRequest
                {
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(16, 30, 0),
                    BreakMinutes = 30,
                    GracePeriodMinutes = 15,
                    MinimumOvertimeMinutes = 45
                }
            }
        };

        var updateResponse = await client
            .PutAsJsonAsync($"/api/v1/WorkSchedules/{created.Id}", updateRequest)
            .ConfigureAwait(false);

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<WorkScheduleDto>().ConfigureAwait(false);

        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated!.Name);
        Assert.Equal(updateRequest.TimeZoneId, updated.TimeZoneId);
        var template = Assert.Single(updated.ShiftTemplates);
        Assert.Equal(DayOfWeek.Wednesday, template.DayOfWeek);
        Assert.Equal(30, template.BreakMinutes);

        var deleteResponse = await client
            .DeleteAsync($"/api/v1/WorkSchedules/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client
            .GetAsync($"/api/v1/WorkSchedules/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task EmployeeSchedulesController_AssignUpdateAndDeleteSchedule()
    {
        using var client = CreateClient();
        var primarySchedule = await CreateWorkScheduleAsync(client, "Core", DayOfWeek.Monday).ConfigureAwait(false);
        var secondarySchedule = await CreateWorkScheduleAsync(client, "Night", DayOfWeek.Sunday).ConfigureAwait(false);
        var employeeId = Guid.NewGuid();

        var createRequest = new CreateEmployeeScheduleRequest
        {
            EmployeeId = employeeId,
            WorkScheduleId = primarySchedule.Id,
            EffectiveFrom = new DateOnly(2025, 1, 1),
            EffectiveTo = new DateOnly(2025, 6, 30)
        };

        var createResponse = await client
            .PostAsJsonAsync("/api/v1/EmployeeSchedules", createRequest)
            .ConfigureAwait(false);

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<EmployeeScheduleDto>().ConfigureAwait(false);

        Assert.NotNull(created);
        Assert.Equal(createRequest.EmployeeId, created!.EmployeeId);
        Assert.Equal(primarySchedule.Id, created.WorkScheduleId);

        var updateRequest = new UpdateEmployeeScheduleRequest
        {
            EmployeeId = employeeId,
            WorkScheduleId = secondarySchedule.Id,
            EffectiveFrom = new DateOnly(2025, 2, 1),
            EffectiveTo = null
        };

        var updateResponse = await client
            .PutAsJsonAsync($"/api/v1/EmployeeSchedules/{created.Id}", updateRequest)
            .ConfigureAwait(false);

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<EmployeeScheduleDto>().ConfigureAwait(false);

        Assert.NotNull(updated);
        Assert.Equal(updateRequest.WorkScheduleId, updated!.WorkScheduleId);
        Assert.Equal(updateRequest.EffectiveFrom, updated.EffectiveFrom);
        Assert.Null(updated.EffectiveTo);

        var list = await client
            .GetFromJsonAsync<EmployeeScheduleDto[]>("/api/v1/EmployeeSchedules")
            .ConfigureAwait(false);

        Assert.Contains(list!, schedule => schedule.Id == created.Id);

        var deleteResponse = await client
            .DeleteAsync($"/api/v1/EmployeeSchedules/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var afterDelete = await client
            .GetAsync($"/api/v1/EmployeeSchedules/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task HolidaysController_CrudFlow_TracksDescriptions()
    {
        using var client = CreateClient();
        var createRequest = new CreateHolidayRequest
        {
            OrganizationId = Guid.NewGuid(),
            Date = new DateOnly(2025, 12, 25),
            Name = "Winter Break",
            IsPaid = true,
            CountryCode = "US",
            Description = "Company-wide shutdown"
        };

        var createResponse = await client
            .PostAsJsonAsync("/api/v1/Holidays", createRequest)
            .ConfigureAwait(false);

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<HolidayDto>().ConfigureAwait(false);

        Assert.NotNull(created);
        Assert.Equal(createRequest.Name, created!.Name);
        Assert.Equal(createRequest.Description, created.Description);

        var updateRequest = new UpdateHolidayRequest
        {
            OrganizationId = createRequest.OrganizationId,
            Date = createRequest.Date,
            Name = "Winter Shutdown",
            IsPaid = false,
            CountryCode = "US",
            Description = "Reduced staffing"
        };

        var updateResponse = await client
            .PutAsJsonAsync($"/api/v1/Holidays/{created.Id}", updateRequest)
            .ConfigureAwait(false);

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<HolidayDto>().ConfigureAwait(false);

        Assert.NotNull(updated);
        Assert.False(updated!.IsPaid);
        Assert.Equal(updateRequest.Name, updated.Name);

        var list = await client
            .GetFromJsonAsync<HolidayDto[]>("/api/v1/Holidays")
            .ConfigureAwait(false);

        Assert.Contains(list!, holiday => holiday.Id == created.Id);

        var deleteResponse = await client
            .DeleteAsync($"/api/v1/Holidays/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var afterDelete = await client
            .GetAsync($"/api/v1/Holidays/{created.Id}")
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        return client;
    }

    private static async Task<WorkScheduleDto> CreateWorkScheduleAsync(HttpClient client, string name, DayOfWeek dayOfWeek)
    {
        var request = new CreateWorkScheduleRequest
        {
            Name = name,
            OrganizationId = Guid.NewGuid(),
            DepartmentId = null,
            IsDefaultForOrganization = false,
            TimeZoneId = "UTC",
            ShiftTemplates = new[]
            {
                new ShiftTemplateRequest
                {
                    DayOfWeek = dayOfWeek,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    BreakMinutes = 45,
                    GracePeriodMinutes = 10,
                    MinimumOvertimeMinutes = 30
                }
            }
        };

        var response = await client
            .PostAsJsonAsync("/api/v1/WorkSchedules", request)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WorkScheduleDto>().ConfigureAwait(false))!;
    }
}
