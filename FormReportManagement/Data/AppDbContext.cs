using FormReportManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FormReportManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FormSubmission> FormSubmissions { get; set; }
}