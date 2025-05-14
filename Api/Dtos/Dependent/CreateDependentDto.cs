using Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Dependent
{
    public class CreateDependentDto
    {
        [Required]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public Relationship? Relationship { get; set; }
    }
}
