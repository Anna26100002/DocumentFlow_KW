using System.Collections.Generic;

namespace DocumentFlow_KW.ViewModels
{
    public class DocumentsViewModel
    {
        public IEnumerable<Document> Documents { get; set; }
        public string Search { get; set; }
        public string Search2 { get; set; }
        public int Count { get; set; }
    }
}
