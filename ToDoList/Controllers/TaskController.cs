using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Helpers;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Models;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var response = await _taskService.Create(model);

        if (response.StatusCode == Domain.Enums.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }

        return BadRequest(new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var pageSize = length != null ? Convert.ToInt32(length) : 0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.PageSize = pageSize;
        filter.Skip = skip;

        var response = await _taskService.GetTasks(filter);

        return Json(new { recordsTotal = response.Total, data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> EndTask(long id)
    {
        var response = await _taskService.EndTask(id);

        if (response.StatusCode == Domain.Enums.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }

        return BadRequest(new { description = response.Description });
    }

    [HttpGet]
    public async Task<IActionResult> GetCompletedTasks()
    {
        var result = await _taskService.GetCompletedTasks();

        return Json(new { data = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await _taskService.CalculateCompletedTasks();
        if (response.StatusCode == Domain.Enums.StatusCode.OK)
        {
            var csvService = new CsvBaseService<IEnumerable<TaskViewModel>>();
            var uploadFile = csvService.UploadFile(response.Data);
            return File(uploadFile, "text/csv", $"Статистика за {DateTime.Now.ToLongDateString()}.csv");
        }

        return BadRequest(new { description = response.Description });
    }
}
