using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using System.Collections.Generic;


    [Table("disk")] // Map to the actual table name
    public class Disk
    {
        [Column("status_id")] // Map to the actual column name in the database
        public int StatusId { get; set; }
        [Column("disk_id")] // Map to the actual column name in the database
        public int DiskId { get; set; }

        [Column("disk_name")] // Map to the actual column name in the database
        public string DiskName { get; set; }

        [Column("disk_type_id")] // Map to the actual column name in the database
        public int DiskTypeId { get; set; }

        [Column("release_date")] // Map to the actual column name in the database
        public DateTime ReleaseDate { get; set; }

        // Foreign Key to Genre
        [Column("genre_id")]
        public int GenreId { get; set; }

        // Navigation Properties
        public Genre Genre { get; set; }
        public Status Status { get; set; }
    }
}
