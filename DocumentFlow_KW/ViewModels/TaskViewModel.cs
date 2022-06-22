using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentFlow_KW.ViewModels
{
    public class TaskViewModel
    {
        public IEnumerable<ViewModels.Task> Tasks { get; set; }
        public string? Search { get; set; }
        public string Search2 { get; set; }
        public int Count { get; set; }
    }
}
