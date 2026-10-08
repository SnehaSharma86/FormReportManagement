using FormReportManagement.Models;
using FormReportManagement.Repositories;

namespace FormReportManagement.Services;

public class FormSubmissionService : IFormSubmissionService
{
    private readonly IFormSubmissionRepository _repository;

    public FormSubmissionService(
        IFormSubmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FormSubmission>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<FormSubmission?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<FormSubmission> CreateAsync(FormSubmission form)
    {
        return await _repository.CreateAsync(form);
    }

    public async Task UpdateAsync(FormSubmission form)
    {
        await _repository.UpdateAsync(form);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
    public async Task<(List<FormSubmission> Items, int TotalCount)> SearchAsync(
    string? search,
    int page,
    int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        return await _repository.SearchAsync(
            search,
            page,
            pageSize);
    }
}