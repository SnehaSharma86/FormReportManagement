using FormReportManagement.Models;
using FormReportManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormReportManagement.Controllers;

public class FormController : Controller
{
    private readonly IFormSubmissionService _service;

    public FormController(
        IFormSubmissionService service)
    {
        _service = service;
    }


    // GET: /Form/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var forms =
            await _service.GetAllAsync();

        return View(forms);
    }


    // GET: /Form/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    // POST: /Form/Create
    [HttpPost]
    public async Task<IActionResult> Create(
        FormSubmission form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        await _service.CreateAsync(form);

        return RedirectToAction("Index");
    }


    // GET: /Form/Edit/7
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var form =
            await _service.GetByIdAsync(id);

        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }


    // POST: /Form/Edit
    [HttpPost]
    public async Task<IActionResult> Edit(
        FormSubmission form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        await _service.UpdateAsync(form);

        return RedirectToAction("Index");
    }


    // POST: /Form/Delete/7
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return RedirectToAction("Index");
    }


    // GET: /Form/GetForms
    [HttpGet]
    public async Task<IActionResult> GetForms()
    {
        var forms =
            await _service.GetAllAsync();

        return Json(forms);
    }
}