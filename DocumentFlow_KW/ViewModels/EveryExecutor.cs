using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentFlow_KW.ViewModels
{
    public class EveryExecutor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Executor { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TimeCompleted { get; set; }
        public string KPI { get; set; }

    }
}
