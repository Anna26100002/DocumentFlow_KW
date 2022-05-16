using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentFlow_KW.ViewModels
{
    public class DocumentViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Fio { get; set; } //Тот, кто создал документ

        [Required]
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public string Executor { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        //public byte[] FileData { get; set; }
        [NotMapped]
        public IFormFile FileData { get; set; }
        public string? TimeCompleted { get; set; }
        public string KPI { get; set; }
    }
}
