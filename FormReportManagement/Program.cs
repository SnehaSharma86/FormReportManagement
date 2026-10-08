using FormReportManagement.Data;
using FormReportManagement.Repositories;
using FormReportManagement.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using FormReportManagement.Middleware;

var builder = WebApplication.CreateBuilder(args);

// MVC + API Controllers
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<
    IFormSubmissionRepository,
    FormSubmissionRepository>();

builder.Services.AddScoped<
    IFormSubmissionService,
    FormSubmissionService>();

// QuestPDF
QuestPDF.Settings.License = LicenseType.Evaluation;

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Error handling
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();