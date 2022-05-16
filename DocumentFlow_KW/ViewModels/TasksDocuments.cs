using System.Collections;
using System.Collections.Generic;

namespace DocumentFlow_KW.ViewModels
{
    public class TasksDocuments
    {
        public IEnumerable<Task> Tasks { get; set; }
        public IEnumerable<Document> Documents { get; set; }
    }
}
