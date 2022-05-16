using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureCreated();
        }

        //public DbSet<User> User { get; set; }
        public DbSet<ViewModels.Task> Tasks { get; set; }
        public DbSet<ViewModels.Document> Documents { get; set; }
        //public DbSet<ViewModels.EveryExecutor> EveryExecutors { get; set; }
        public DbSet<DocumentViewModel> Files { get; set; }
    }
}
