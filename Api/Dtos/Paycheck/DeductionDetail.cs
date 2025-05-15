namespace Api.Dtos.Paycheck
{
    public class DeductionDetail
    {
        public string Description { get; set; } = null!;
        public decimal AnnualAmount { get; set; }
        public decimal PerPaycheckAmount { get; set; }
    }
}
