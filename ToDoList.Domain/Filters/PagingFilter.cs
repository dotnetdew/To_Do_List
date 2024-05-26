using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Filters;

public class PagingFIlter
{
    public int PageSize { get; set; }
    public int Skip { get; set; }
}
