using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Enums;

public enum StatusCode
{
    TaskAlreadyCreated = 1,
    TaskNotFound = 2,
    OK = 200,
    InternalServerError = 500
}
