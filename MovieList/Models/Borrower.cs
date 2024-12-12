using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace MovieList.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Borrower
    {
        [Column("borrower_id")]
        public int BorrowerId { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LName { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be 10 digits.")]
        [Column("phone_num")] // Map to the "phone" column
        public string PhoneNum { get; set; }
      
        

        // Navigation Property
        public ICollection<DiskHasBorrower> DiskHasBorrowers { get; set; }
    }
}
