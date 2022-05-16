using DocumentFlow_KW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public int Year { get; set; }
        public string Fio { get; set; }
        public string Position { get; set; }
        public string Chief { get; set; }
        public IEnumerable<User> FioUsers { get; set; }
        public string FioPosition { get; set; }
    }
}
