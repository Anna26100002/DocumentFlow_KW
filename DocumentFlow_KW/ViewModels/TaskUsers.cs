using DocumentFlow_KW.Models;
using System.Collections.Generic;

namespace DocumentFlow_KW.ViewModels
{
    public class TaskUsers
    {
        public IEnumerable<User> FioUsers { get; set; }
        public Task Task { get; set; }

    }
}
