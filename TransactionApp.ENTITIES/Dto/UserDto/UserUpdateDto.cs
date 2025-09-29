
using System.ComponentModel.DataAnnotations;

namespace TransactionApp.ENTITIES.Dto.UserDto
{
    public class UserUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than zero")]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public required string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string? Email { get; set; }
    }
}
