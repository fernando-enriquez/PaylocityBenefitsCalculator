using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Interfaces;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Rule> _ruleRepository;
    private readonly IMapper _mapper;

    public EmployeesController(IRepository<Employee> employeeRepository, IMapper mapper, IRepository<Rule> ruleRepository)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _ruleRepository = ruleRepository;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, e => e.Dependents);
        var employeeDTO = _mapper.Map<GetEmployeeDto>(employee);

        var result = new ApiResponse<GetEmployeeDto>
        {
            Data = employeeDTO,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        //task: use a more realistic production approach
        var employeeList = await _employeeRepository.GetAllAsync(e => e.Dependents);

        var employees = _mapper.Map<List<GetEmployeeDto>>(employeeList);

        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = employees,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Save one employee")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<bool>>> Save([FromBody] CreateEmployeeDto body)
    {
        //get validations rules
        var domesticPartnerLimitRule = await _ruleRepository.GetFirstOrDefaultAsync(x => x.Concept == "domesticPartnerQuantityLimit");
        var childrenQuantityLimitRule = await _ruleRepository.GetFirstOrDefaultAsync(x => x.Concept == "childrenQuantityLimit");


        if (domesticPartnerLimitRule == null)
        {
            return BadRequest("Rule for domestic partner/spouse limit not configured.");
        }

        if (childrenQuantityLimitRule == null)
        {
            return BadRequest("Rule for children quantity limit not configured.");
        }

        //Validation for domestic partner or spouse limit

        //Validate only if the rule indicates an specific maximun (not unlimited)
        if (domesticPartnerLimitRule.Value.HasValue && domesticPartnerLimitRule.Unlimited == false && body.Dependents != null)
        {
            //Validate the quantity of type spouse/domestic partner
            var count = body.Dependents.Count(x => x.Relationship == Relationship.DomesticPartner || x.Relationship == Relationship.Spouse);

            if (count > domesticPartnerLimitRule.Value)
            {
                return BadRequest($"The number of domestic partner or spouses exceeds the allowed limit of {domesticPartnerLimitRule.Value}");
            }
        }

        //Validation for children limit
        //Validate only if the rule indicates an specific maximun (not unlimited)
        if (childrenQuantityLimitRule.Value.HasValue && childrenQuantityLimitRule.Unlimited == false && body.Dependents != null)
        {
            //If the employee has dependents validate the quantity of type spouse/domestic partner
            var count = body.Dependents.Count(x => x.Relationship == Relationship.Child);

            if (count > childrenQuantityLimitRule.Value)
            {
                return BadRequest($"The number of children exceeds the allowed limit of {childrenQuantityLimitRule.Value}");
            }
        }

        //save an employee
        Employee employee = new Employee
        {
            FirstName = body.FirstName,
            LastName = body.LastName,
            Salary = body.Salary.Value,
            DateOfBirth = body.DateOfBirth.Value
        };

        await _employeeRepository.AddAsync(employee);

        await _employeeRepository.SaveChangesAsync();




        var result = new ApiResponse<bool>
        {
            Data = true,
            Success = true
        };

        return result;
    }
}
