using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Response;

public class DataTableResult
{
    public object Data { get; set; }
    public int Total { get; set; }
}
