using Api.Dtos.Employee;

namespace Api.Dtos.Paycheck
{
    public class PaycheckDto
    {
        public GetEmployeeDto Employee {  get; set; }
        public decimal GrossPayment { get; set; }
        public decimal TotalDeductionPerPayCheck { get; set; }  
        public decimal NetPayment {  get; set; }

        public ICollection<DeductionDetail> Details { get; set; } = new List<DeductionDetail>();
    }
}
