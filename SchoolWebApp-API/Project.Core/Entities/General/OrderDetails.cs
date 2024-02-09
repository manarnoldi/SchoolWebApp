using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Core.Entities.General
{
    [Table("OrderDetails")]
    public class OrderDetails : Base
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public decimal SellingPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [StringLength(maximumLength: 350)]
        public string? Description { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

    }
}
