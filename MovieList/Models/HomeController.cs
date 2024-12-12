using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{


    public class BorrowedDiskViewModel
    {
        public string DiskName { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string BorrowerName { get; set; } // Optional
    }
}
