using FormReportManagement.Models;

namespace FormReportManagement.Repositories;

public interface IFormSubmissionRepository
{
    Task<List<FormSubmission>> GetAllAsync();

    Task<FormSubmission?> GetByIdAsync(int id);

    Task<FormSubmission> CreateAsync(FormSubmission form);

    Task UpdateAsync(FormSubmission form);

    Task DeleteAsync(int id);

    Task<(List<FormSubmission> Items, int TotalCount)> SearchAsync(
        string? search,
        int page,
        int pageSize);
}