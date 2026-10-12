using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystemMVC.Models
{
    public class Worker
    {
        [Key]
        public int WorkerID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; }
    }

}