using DocumentFlow_KW.Models;
using System.Collections.Generic;

namespace DocumentFlow_KW.ViewModels
{
    public class DocumentUsers
    {
        public IEnumerable<User> FioUsers { get; set; }
        public Document Document { get; set; }

    }
}
