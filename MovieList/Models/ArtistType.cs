using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieList.Models
{


    public class ArtistType
    {
        public int ArtistTypeId { get; set; }
        public string Description { get; set; }

        // Navigation property for artists of this type
        public ICollection<Artist> Artists { get; set; }
    }
}
