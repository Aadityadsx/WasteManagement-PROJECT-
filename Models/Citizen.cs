
using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystemMVC.Models
{
    public class Citizen
    {
        [Key]
        public int CitizenID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}