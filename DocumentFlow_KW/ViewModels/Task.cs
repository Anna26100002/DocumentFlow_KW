using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DocumentFlow_KW.ViewModels
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Topic { get; set; }
        public DateTime CreationDate { get; set; }

        public DateTime EndDate { get; set; }
        //public string Responsible { get; set; } //Тот, кто отправил документ
        //public virtual User User { get; set; } //Тот, кто отправил документ
        public string Fio { get; set; } //Тот, кто создал документ

        public string Executor { get; set; } //Исполнитель

        [Required]
        public string Priority { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public string? TimeCompleted { get; set; }
        public string KPI { get; set; }
        //public IEnumerable<User> FioUsers { get; set; }
    }
}
