using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Enums;
using ToDoList.Domain.Extensions;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<TaskEntity> _taskRepository;
    private ILogger<TaskService> _logger;
    public TaskService(IBaseRepository<TaskEntity> taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.CreatedAt.Date == DateTime.Today)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsDone = x.IsDone == true ? "Done" : "Not Done",
                    Priority = x.Priority.ToString(),
                    CreatedAt = x.CreatedAt.ToString(CultureInfo.InvariantCulture)
                }).ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Description = "this kind of task already craeted",
                StatusCode = StatusCode.TaskAlreadyCreated
            };
        }
    }

    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();

            _logger.LogInformation($"request for creating task - {model.Name}");

            var task = await _taskRepository.GetAll()
                .Where(x => x.CreatedAt.Date == DateTime.Today)
                .FirstOrDefaultAsync(x => x.Name == model.Name);

            if (task != null)
            {
                return new BaseResponse<TaskEntity>
                {
                    Description = "this kind of task already craeted",
                    StatusCode = StatusCode.TaskAlreadyCreated
                };
            }

            task = new TaskEntity()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                Priority = model.Priority,
                IsDone = false
            };

            await _taskRepository.Create(task);

            _logger.LogInformation($"Task successfully created: {task.Name} {task.CreatedAt}");
            return new BaseResponse<TaskEntity>()
            {
                Description = "Task successfully created",
                StatusCode = StatusCode.OK,
                Data = task
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.Create]: {ex.Message}");

            return new BaseResponse<TaskEntity>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id)
    {
        try
        {
            var task = await _taskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return new BaseResponse<bool>()
                {
                    StatusCode = StatusCode.TaskNotFound,
                    Description = "Задача не найдена!"
                };
            }

            task.IsDone = true;

            await _taskRepository.Update(task);

            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.OK,
                Description = "Задача завершена!"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");

            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<CompletedTaskViewModel>>> GetCompletedTasks()
    {
        try
        {
            var tasks = _taskRepository.GetAll()
                .Where(x => x.IsDone)
                .Where(x => x.CreatedAt.Date == DateTime.Today)
                .Select(x => new CompletedTaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                });

            return new BaseResponse<IEnumerable<CompletedTaskViewModel>>()
            {
                StatusCode = StatusCode.OK,
                Data = tasks
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");

            return new BaseResponse<IEnumerable<CompletedTaskViewModel>>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => !x.IsDone)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова",
                    Priority = x.Priority.GetDisplayName(),
                    CreatedAt = x.CreatedAt.ToLongDateString()
                })
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var count = _taskRepository.GetAll().Count(x => !x.IsDone);

            return new DataTableResult()
            {
                Data = tasks,
                Total = count
            };
        }
        catch(Exception ex)
        {
            _logger.LogError($"{ex.Message}");

            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }
}
