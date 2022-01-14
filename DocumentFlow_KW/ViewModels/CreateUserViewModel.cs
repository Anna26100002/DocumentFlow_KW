using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.ViewModels
{
    public class CreateUserViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int Year { get; set; }
        public string Fio { get; set; }
        public string Position { get; set; }

        public List<CreateUserViewModel> List { get; set; }
    }
}
