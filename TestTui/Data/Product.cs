using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        public string Code { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("start_validity_date")]
        public DateTime StartValidityDate { get; set; }

        [Required]
        [Column("end_validity_date")]
        public DateTime EndValidityDate { get; set; }
    }
}
