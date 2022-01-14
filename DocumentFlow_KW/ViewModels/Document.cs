using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.ViewModels
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Topic { get; set; }
        public DateTime CreationDate { get; set; }
        //public string Responsible { get; set; } //Тот, кто отправил документ
        public virtual User User { get; set; } //Тот, кто отправил документ
        public string Executor { get; set; } //Исполнитель
        public string Priority { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}
