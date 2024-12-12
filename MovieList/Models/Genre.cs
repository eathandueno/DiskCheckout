using System.ComponentModel.DataAnnotations;
using System;
namespace MovieList.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using System.ComponentModel.DataAnnotations.Schema;

  

    [Table("genre")]
    public class Genre
    {
        [Column("genre_id")] // Map 'GenreId' to 'genre_id'
        public int GenreId { get; set; }

        public string Description { get; set; }


    }
}
