using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces;

public interface ITaskService
{
    Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model);
    Task<DataTableResult> GetTasks(TaskFilter filter);
    Task<IBaseResponse<bool>> EndTask(long id);
    Task<IBaseResponse<IEnumerable<CompletedTaskViewModel>>> GetCompletedTasks();
    Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks();
}
