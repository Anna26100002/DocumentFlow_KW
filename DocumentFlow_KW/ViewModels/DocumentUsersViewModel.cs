using DocumentFlow_KW.Models;
using System.Collections.Generic;

namespace DocumentFlow_KW.ViewModels
{
    public class DocumentUsersViewModel
    {
        public IEnumerable<User> FioUsers { get; set; }
        public DocumentViewModel DocumentViewModel { get; set; }
    }
}
