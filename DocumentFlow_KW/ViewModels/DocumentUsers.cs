using DocumentFlow_KW.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.ViewModels
{
    [Keyless]
    public class DocumentUsers
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public IEnumerable<User> Users {get; set;}
        public Document Documents { get; set; }

    }
}
