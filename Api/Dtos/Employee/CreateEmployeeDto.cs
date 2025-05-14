using Api.Dtos.Dependent;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Employee
{
    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal? Salary { get; set; }
        
        [Required]
        public DateTime? DateOfBirth { get; set; }

        public List<CreateDependentDto>? Dependents { get; set; }
    }
}
