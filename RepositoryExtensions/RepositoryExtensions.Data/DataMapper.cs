using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace RepositoryExtensions.Data
{
    public class DataMapper
    {
        private const int DEPTH = 2;

        public void Configure()
        {
            Mapper
                .CreateMap<Data.Models.Employee, Core.IManager>()
                .MaxDepth(DEPTH)
                .ConstructUsing(CreateDomainManager)
                .ReverseMap();
            Mapper
                .CreateMap<Data.Models.Employee, Core.IEmployee>()
                .MaxDepth(DEPTH)
                .ConstructUsing(CreateDomainEmployee)
                .ReverseMap();
            Mapper
                .CreateMap<Data.Models.Employee, Core.Model.Employee>()
                .MaxDepth(DEPTH)
                .ConstructUsing(CreateConcreteEmployee)
                .ReverseMap();
        }

        private Core.IManager CreateDomainManager(Data.Models.Employee employee)
        {
            return new Core.Model.Employee(employee.Id, 
                employee.Name, 
                Mapper.Map<Core.IManager>(employee.Manager), 
                Mapper.Map<Core.IEmployee[]>(employee.Employees));
        }

        private Core.IEmployee CreateDomainEmployee(Data.Models.Employee employee)
        {
            return new Core.Model.Employee(employee.Id,
                employee.Name,
                Mapper.Map<Core.IManager>(employee.Manager));
        }

        private Core.Model.Employee CreateConcreteEmployee(Data.Models.Employee employee)
        {
            return new Core.Model.Employee(employee.Id,
                employee.Name,
                Mapper.Map<Core.IManager>(employee.Manager),
                Mapper.Map<Core.IEmployee[]>(employee.Employees));
        }
    }
}
