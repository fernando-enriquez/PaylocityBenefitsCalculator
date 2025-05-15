using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Api.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string Concept { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Value { get; set; }
        public bool Unlimited { get; set; }
    }
}
