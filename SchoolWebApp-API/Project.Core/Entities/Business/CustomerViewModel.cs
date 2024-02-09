using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(maximumLength: 100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;
        [Required, DataType(DataType.EmailAddress), StringLength(maximumLength: 100, MinimumLength = 5)]
        public string Email { get; set; } = string.Empty;
        public decimal? Balance { get; set; } = 0;
    }
}
