using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }


        [Required]
        public string Name { get; set; }


        [Required]
        public DateTime StartValidityDate { get; set; }


        [Required]
        public DateTime EndValidityDate { get; set; }
    }
}
