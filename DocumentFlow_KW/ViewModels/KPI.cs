using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentFlow_KW.ViewModels
{
    public class KPI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string user { get; set; }
        public int? TasksPriorityInTerm { get; set; }
        public int? TasksPriority { get; set; }
        public double? KPI1 { get; set; }
        public int? DocumentsPriorityInTerm { get; set; }
        public int? DocumentsPriority { get; set; }
        public double? KPI2 { get; set; }
        public int? TasksInTerm { get; set; }
        public double? KPI3 { get; set; }
        public int? DocumentsInTerm { get; set; }
        public double? KPI4 { get; set; }
        public int? TasksNotTerm { get; set; }
        public double? KPI5 { get; set; }
        public int? DocumentsNotTerm { get; set; }
        public double? KPI6 { get; set; }
        public int? Tasks { get; set; }
        public int? Documents { get; set; }
        public double? KPIgeneral { get; set; }


    }
}
