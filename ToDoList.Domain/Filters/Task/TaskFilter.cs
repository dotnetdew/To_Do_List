using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Enums;

namespace ToDoList.Domain.Filters.Task;

public class TaskFilter : PagingFIlter
{
    public string Name { get; set; }
    public Priority? Priority { get; set; }
}
