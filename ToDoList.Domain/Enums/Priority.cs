using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Enums;

public enum Priority
{
    [Display(Name = "Простая")]
    Easy = 1,
    [Display(Name = "Важная")]
    Medium = 2,
    [Display(Name = "Критичная")]
    Hard = 3
}
