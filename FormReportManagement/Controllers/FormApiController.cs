using FormReportManagement.DTOs;
using FormReportManagement.Models;
using FormReportManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;

namespace FormReportManagement.Controllers;

[ApiController]
[Route("api/forms")]
public class FormApiController : ControllerBase
{
    private readonly IFormSubmissionService _service;
    private readonly ILogger<FormApiController> _logger;

    public FormApiController(
        IFormSubmissionService service,
        ILogger<FormApiController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET: api/forms
    [HttpGet]
    public async Task<IActionResult> GetForms()
    {
        _logger.LogInformation("GetForms API called.");

        var forms = await _service.GetAllAsync();

        var response = forms.Select(form => new FormSubmissionResponseDto
        {
            Id = form.Id,
            Name = form.Name,
            Email = form.Email,
            Phone = form.Phone
        }).ToList();

        _logger.LogInformation(
            "Forms retrieved successfully. Count: {Count}",
            response.Count);

        return Ok(response);
    }


    // POST: api/forms
    [HttpPost]
    public async Task<IActionResult> CreateForm(
        FormSubmissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // DTO → Entity
        var form = new FormSubmission
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone
        };

        var createdForm = await _service.CreateAsync(form);

        // Entity → Response DTO
        var response = new FormSubmissionResponseDto
        {
            Id = createdForm.Id,
            Name = createdForm.Name,
            Email = createdForm.Email,
            Phone = createdForm.Phone
        };

        return CreatedAtAction(
            nameof(GetFormById),
            new { id = response.Id },
            response);
    }


    // GET: api/forms/7
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormById(int id)
    {
        var form = await _service.GetByIdAsync(id);

        if (form == null)
            return NotFound();

        // Entity → Response DTO
        var response = new FormSubmissionResponseDto
        {
            Id = form.Id,
            Name = form.Name,
            Email = form.Email,
            Phone = form.Phone
        };

        return Ok(response);
    }


    // PUT: api/forms/7
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateForm(
        int id,
        FormSubmissionUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingForm = await _service.GetByIdAsync(id);

        if (existingForm == null)
            return NotFound();

        // Update Entity using Update DTO
        existingForm.Name = dto.Name;
        existingForm.Email = dto.Email;
        existingForm.Phone = dto.Phone;

        await _service.UpdateAsync(existingForm);

        // Entity → Response DTO
        var response = new FormSubmissionResponseDto
        {
            Id = existingForm.Id,
            Name = existingForm.Name,
            Email = existingForm.Email,
            Phone = existingForm.Phone
        };

        return Ok(response);
    }


    // DELETE: api/forms/7
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(int id)
    {
        var existingForm = await _service.GetByIdAsync(id);

        if (existingForm == null)
            return NotFound();

        await _service.DeleteAsync(id);

        return Ok(new
        {
            message = "Form deleted successfully"
        });
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search(
    string? search,
    int page = 1,
    int pageSize = 10)
    {
        var result = await _service.SearchAsync(
            search,
            page,
            pageSize);

        var response = result.Items
            .Select(form => new FormSubmissionResponseDto
            {
                Id = form.Id,
                Name = form.Name,
                Email = form.Email,
                Phone = form.Phone
            })
            .ToList();

        return Ok(new
        {
            data = response,
            totalCount = result.TotalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(
                (double)result.TotalCount / pageSize)
        });
    }

    // GET: api/forms/export/csv
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportCsv()
    {
        var forms = await _service.GetAllAsync();

        var csv = new StringBuilder();

        csv.AppendLine("Id,Name,Email,Phone");

        foreach (var form in forms)
        {
            csv.AppendLine(
                $"{form.Id},{form.Name},{form.Email},{form.Phone}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return File(
            bytes,
            "text/csv",
            "FormSubmissions.csv");
    }
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "API is running"
        });
    }


    // GET: api/forms/export/excel
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var forms = await _service.GetAllAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Forms");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Email";
        worksheet.Cell(1, 4).Value = "Phone";

        int row = 2;

        foreach (var form in forms)
        {
            worksheet.Cell(row, 1).Value = form.Id;
            worksheet.Cell(row, 2).Value = form.Name;
            worksheet.Cell(row, 3).Value = form.Email;
            worksheet.Cell(row, 4).Value = form.Phone;

            row++;
        }

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var bytes = stream.ToArray();

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "FormSubmissions.xlsx");
    }


    // GET: api/forms/export/pdf
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var forms = await _service.GetAllAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("Form Submissions Report")
                    .FontSize(20)
                    .Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text("Id")
                            .Bold();

                        header.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text("Name")
                            .Bold();

                        header.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text("Email")
                            .Bold();

                        header.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text("Phone")
                            .Bold();
                    });

                    foreach (var form in forms)
                    {
                        table.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text(form.Id.ToString());

                        table.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text(form.Name);

                        table.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text(form.Email);

                        table.Cell()
                            .Border(1)
                            .Padding(5)
                            .Text(form.Phone);
                    }
                });
            });
        });

        var pdfBytes = document.GeneratePdf();

        return File(
            pdfBytes,
            "application/pdf",
            "FormSubmissions.pdf");
    }
}