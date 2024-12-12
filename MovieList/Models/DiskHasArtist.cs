using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class DiskHasArtist
    {
        [Column("disk_has_artist_id")] // Map to the actual column name
        public int DiskHasArtistId { get; set; }

        public int DiskId { get; set; }
        public Disk Disk { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
