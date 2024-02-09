using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Core.Entities.General
{
    [Table("Orders")]
    public class Order : Base
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public decimal TotalBill { get; set; }
        [Required]
        public int TotalQuantity { get; set; }
        [Required]
        public DateTime ProcessingData { get; set; }
        [StringLength(maximumLength: 350)]
        public string? Description { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
