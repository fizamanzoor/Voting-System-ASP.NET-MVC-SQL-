using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Models
{
    public class VotingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string Contact { get; set; }

        [Required]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120.")]
        public int Age { get; set; }

        [Required]
        [RegularExpression(@"\d{5}-\d{7}-\d", ErrorMessage = "CNIC format must be 12345-1234567-1.")]
        public string CNIC { get; set; }


        public string AssemblyType { get; set; }

        public string SelectedParty { get; set; }
    }
}
