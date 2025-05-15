using Api.Dtos.Employee;
using Api.Dtos.Paycheck;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaycheckController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Rule> _ruleRepository;
        private readonly IRepository<Dependent> _dependentRepository;
        private readonly IMapper _mapper;

        public PaycheckController(IRepository<Employee> employeeRepository, IMapper mapper, IRepository<Rule> ruleRepository, IRepository<Dependent> dependentRepository)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _ruleRepository = ruleRepository;
            _dependentRepository = dependentRepository;
        }


        [SwaggerOperation(Summary = "Get paycheck by employee")]
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<ApiResponse<PaycheckDto>>> Get(int employeeId)
        {
            //This method is intented to calculate a paycheck for an employee after some benefits discounts
        
            //parameters involved in the paycheck calculation
           
            //annual gross salary for the employee
            decimal employeeAnnualSalaryGross = 0;

            //number of paychecks the employee receives in a year
            int paychecksPerYear = 0;

            //benefits base cost per employee per month
            decimal employeeBaseCost = 0;

            //employee´s dependent base cost per month
            decimal dependentBaseCost = 0;

            //threshold salary: if the employee earn more than this, amount an additional fee is applied
            decimal thresholdAnnualSalary = 0;

            // fee applied if the annual salary exceeds the threshold.
            // example: 2 means 2% of the salary will be added as extra cost.
            decimal thresholdAnnualSalaryFee = 0;

            //age threshold for applying an extra fee to older dependents
            decimal thresholdAge = 0;

            //monthly extra cost for dependents who exceed the age threshold.
            decimal thresholdAgeFee = 0;

            //get the rules for the paycheck calculation parameters
            //All of these values are stored in the database under a Rules table witrh the following structure

            //{
            //    Concept:The identifier of the parameter,
            //    Value: numerical value to be used
            //}

           
            //Paychecks per year
            try
            {
                Rule paychecksPerYearRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "paychecksPerYear");
                paychecksPerYear = (int)paychecksPerYearRule.Value!;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            
            //Employee´s base cost
            try
            {
                Rule employeeBaseCostRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "employeeBaseCost");
                employeeBaseCost = employeeBaseCostRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //Dependent base cost
            try
            {
                Rule dependentBaseCostRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "dependentBaseCost");
                dependentBaseCost = dependentBaseCostRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //threshold salary
            try
            {
                Rule thresholdAnnualSalaryRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "thresholdAnnualSalary");
                thresholdAnnualSalary = thresholdAnnualSalaryRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //fee applied if the annual salary exceeds the threshold.
            try
            {
                Rule thresholdAnnualSalaryFeeRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "thresholdAnnualSalaryFee");
                thresholdAnnualSalaryFee = thresholdAnnualSalaryFeeRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //age threshold
            try
            {
                Rule thresholdAgeRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "thresholdAge");
                thresholdAge = thresholdAgeRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //monthly extra cost for dependents who exceed the age threshold.
            try
            {
                Rule thresholdAgeFeeRule = await RuleFetcher.GetRequiredRuleAsync(_ruleRepository, "thresholdAgeFee");
                thresholdAgeFee = thresholdAgeFeeRule.Value.Value;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            //get the employee record an its annualSalary
            var employee = await _employeeRepository.GetByIdAsync(employeeId, e => e.Dependents);

            if (employee == null)
            {
                return BadRequest($"Employee with ID {employeeId} was not found.");
            }

            if (employee.Salary <= 0 )
            {
                return BadRequest($"Employee '{employee.FirstName} {employee.LastName}' does not have a valid salary configured.");
            }

            employeeAnnualSalaryGross = employee.Salary;

            //Make the calculations

            //anual employee base cost
            decimal annualEmployeeBaseCost = employeeBaseCost * 12;

            //if the annual salary exceeds the salary threshold, and additional yearly cost is applied
            decimal salaryOverThresholdCostPerYear = 0;

            if (employeeAnnualSalaryGross > thresholdAnnualSalary)
            {
                //get a percent of the employee salary
                salaryOverThresholdCostPerYear = (employeeAnnualSalaryGross * thresholdAnnualSalaryFee) / 100;
            }


            //get the cost for each dependent
            decimal employeeDependentCostPerMonth = 0;
            decimal employeeDependentOverYearThresholdCostPerMonth = 0;

            foreach (var dependent in employee.Dependents)
            {
                //every dependent has a monthly base cost regardless the type of relationship
                employeeDependentCostPerMonth += dependentBaseCost;

                //if some of the dependent exceeds the age threshold, an additional monthly cost is applied
                //first, calculate the dependent´s age at the start of the current year, based on their birthdate
                int age = AgeUtils.GetAgeAtStartOfYear(dependent.DateOfBirth);

                if (age > thresholdAge)
                {
                    employeeDependentOverYearThresholdCostPerMonth += thresholdAgeFee;
                }
            }

            //convert the monthly cost to annual
            decimal employeeDependentCostPerYear = employeeDependentCostPerMonth * 12;
            decimal employeeDependentOverYearThresholdCostPerYear = employeeDependentOverYearThresholdCostPerMonth * 12;

            //get the total discount per year
            decimal totalDiscountPerYear = annualEmployeeBaseCost + salaryOverThresholdCostPerYear + employeeDependentCostPerYear + employeeDependentOverYearThresholdCostPerYear;

            //get the gross salary per paycheck
            decimal employeeSalaryGrossPerCheck = employeeAnnualSalaryGross / paychecksPerYear;

            //get the discount per paycheck
            decimal discountPerPaycheck = totalDiscountPerYear / paychecksPerYear;

            //get the net salary per paycheck 
            decimal employeeSalaryNetPerCheck = employeeSalaryGrossPerCheck - discountPerPaycheck;

            // fill the paycheck DTO
            var paycheckDTO = new PaycheckDto
            {
                Employee = _mapper.Map<GetEmployeeDto>(employee),
                GrossPayment = Math.Round(employeeSalaryGrossPerCheck,2),
                TotalDeductionPerPayCheck = Math.Round(discountPerPaycheck,2),
                NetPayment = Math.Round(employeeSalaryNetPerCheck,2),

                Details = new List<DeductionDetail>
                {
                    new DeductionDetail
                    {
                        Description = "Base employee cost",
                        AnnualAmount = Math.Round(annualEmployeeBaseCost,2),
                        PerPaycheckAmount = Math.Round(annualEmployeeBaseCost / paychecksPerYear),
                    },
                    new DeductionDetail
                    {
                        Description = $"Dependent cost ({employee.Dependents.Count} dependents)",
                        AnnualAmount = Math.Round(employeeDependentCostPerYear,2),
                        PerPaycheckAmount = Math.Round(employeeDependentCostPerYear / paychecksPerYear,2),
                    },
                    new DeductionDetail
                    {
                        Description = $"Extra cost for dependents over {thresholdAge}",
                        AnnualAmount = Math.Round(employeeDependentOverYearThresholdCostPerYear, 2),
                        PerPaycheckAmount = Math.Round(employeeDependentOverYearThresholdCostPerYear / paychecksPerYear,2),
                    },
                    new DeductionDetail
                    {
                        Description = $"Extra cost ({thresholdAnnualSalaryFee}% of the annual gross salary) for employee´s salary over {thresholdAnnualSalary}",
                        AnnualAmount = Math.Round(salaryOverThresholdCostPerYear, 2),
                        PerPaycheckAmount = Math.Round(salaryOverThresholdCostPerYear / paychecksPerYear,2)
                    }
                }
            };

            var result = new ApiResponse<PaycheckDto>
            {
                Data = paycheckDTO,
                Success = true
            };

            return result;
        }
    }
}
