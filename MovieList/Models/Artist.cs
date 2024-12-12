using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Artist
    {
        public int ArtistId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }

        public int ArtistTypeId { get; set; }
        public ArtistType ArtistType { get; set; }

        // Navigation property for disks this artist contributed to
        public ICollection<DiskHasArtist> DiskHasArtists { get; set; }
    }
}
