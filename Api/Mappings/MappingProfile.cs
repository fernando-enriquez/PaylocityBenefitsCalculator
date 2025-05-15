using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using AutoMapper;

namespace Api.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<Employee, GetEmployeeDto>();
            CreateMap<Dependent, GetDependentDto>();

            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<CreateDependentDto, Dependent>();
        }
    }
}
