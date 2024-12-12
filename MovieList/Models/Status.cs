using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieList.Models
{
    using System.ComponentModel.DataAnnotations;

    [Table("status")]
    public class Status
    {
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
