using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.ViewModels.Task;

public class CompletedTaskViewModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
