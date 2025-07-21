using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruVa.Ordering.DataAccess.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderNumber { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = [];
    }
}
