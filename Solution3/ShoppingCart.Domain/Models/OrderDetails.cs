using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShoppingCart.Domain.Models
{
    public class OrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ForeignKey("Product")]
        public Guid ProductFK { get; set; }
        [Required]
        public virtual Product Product { get; set; }

        [ForeignKey("Order")]
        public Guid OrderFK { get; set; }
        [Required]
        public virtual Order Order { get; set; }

        [Required]
        [DefaultValue(1)]
        public int Quantity { get; set; }

        public double Price { get; set; }


    }
}
