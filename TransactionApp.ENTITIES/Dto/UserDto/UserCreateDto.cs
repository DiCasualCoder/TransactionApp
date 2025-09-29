using System.ComponentModel.DataAnnotations;

namespace TransactionApp.ENTITIES.Dto.UserDto
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public required string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string? Email { get; set; }
    }
}
