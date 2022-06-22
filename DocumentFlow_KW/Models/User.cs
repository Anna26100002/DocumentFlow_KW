using DocumentFlow_KW.ViewModels;
using DocumentFlow_KW.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentFlow_KW.Models
{
    public class User : IdentityUser
    {
        public string Login { get; set; }

        public string Fio { get; set; }
        public string Position { get; set; }
        public int Year { get; set; }

        public string Chief { get; set; }

        public double? KPI { get; set; }
        //public virtual ICollection<Document> Documents { get; set; }
    }
}
