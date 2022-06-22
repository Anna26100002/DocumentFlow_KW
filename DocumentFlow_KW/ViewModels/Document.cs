using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Policy;

namespace DocumentFlow_KW.ViewModels
{
    public class Document
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Fio { get; set; } //Тот, кто создал документ
        //public string CurrentExecutor { get; set; } //Исполнитель
        [Required]
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Executor { get; set; }
        public string? FileName { get; set; }
        public int? Count { get; set; }
        public string Comment { get; set; }
        public string? TimeCompleted { get; set; }
        public string KPI { get; set; }
        public string Term { get; set; }
        public string? UserName { get; set; }
        //public IEnumerable<EveryExecutor> EveryExecutor { get; set; }
    }
}
