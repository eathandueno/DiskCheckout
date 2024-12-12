using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DiskType
    {
        public int DiskTypeId { get; set; }
        public string Description { get; set; }

        // Navigation property for disks of this type
        public ICollection<Disk> Disks { get; set; }
    }
}
