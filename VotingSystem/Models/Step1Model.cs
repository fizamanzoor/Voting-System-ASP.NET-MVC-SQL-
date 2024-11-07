using System.ComponentModel.DataAnnotations;


namespace VotingSystem.Models
{
    public class Step1Model
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "CNIC is required.")]
        [RegularExpression(@"\d{5}-\d{7}-\d", ErrorMessage = "CNIC format must be 12345-1234567-1.")]
        public string CNIC { get; set; }
    }
}
