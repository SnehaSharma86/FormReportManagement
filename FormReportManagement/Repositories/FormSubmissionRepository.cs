using FormReportManagement.Data;
using FormReportManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FormReportManagement.Repositories;

public class FormSubmissionRepository : IFormSubmissionRepository
{
    private readonly AppDbContext _context;

    public FormSubmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FormSubmission>> GetAllAsync()
    {
        return await _context.FormSubmissions.ToListAsync();
    }

    public async Task<FormSubmission?> GetByIdAsync(int id)
    {
        return await _context.FormSubmissions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FormSubmission> CreateAsync(FormSubmission form)
    {
        _context.FormSubmissions.Add(form);

        await _context.SaveChangesAsync();

        return form;
    }

    public async Task UpdateAsync(FormSubmission form)
    {
        _context.FormSubmissions.Update(form);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var form = await _context.FormSubmissions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (form != null)
        {
            _context.FormSubmissions.Remove(form);

            await _context.SaveChangesAsync();
        }
    }
    public async Task<(List<FormSubmission> Items, int TotalCount)> SearchAsync(
    string? search,
    int page,
    int pageSize)
    {
        var query = _context.FormSubmissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Email.Contains(search) ||
                x.Phone.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}