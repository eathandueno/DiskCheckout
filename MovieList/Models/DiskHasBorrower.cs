using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieList.Models
{

    [Table("disk_has_borrower")]
    public class DiskHasBorrower
    {
        [Column("disk_has_borrower_id")] // Map to the actual column name
        public int DiskHasBorrowerId { get; set; }

        [Column("borrower_id")] // Foreign key for Borrower
        public int BorrowerId { get; set; }

        [Column("disk_id")] // Foreign key for Disk
        public int DiskId { get; set; }

        [Column("borrowed_date")] // Match the database column name
        public DateTime BorrowedDate { get; set; }

        [Column("due_date")] // Match the database column name
        public DateTime DueDate { get; set; }

        // Optional relationships
        public Borrower Borrower { get; set; }
        public Disk Disk { get; set; }
    }
}
