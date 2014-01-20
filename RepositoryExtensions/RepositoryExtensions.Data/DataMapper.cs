using AutoMapper;
using QueryMapper;
using System.Collections.Generic;

namespace RepositoryExtensions.Data
{
    public class DataMapper
    {
        private const int DEPTH = 2;

        public void Configure()
        {
            Mapper
                .CreateMap<Data.Models.Employee, Core.IManager>()
                .ForMember(d => d.Employees, d => d.Ignore())
                .AfterMap((s, d) => {
                    if (s.Employees != null)
                        s.Employees.Each(e => d.GainEmployee(Mapper.Map<Core.IEmployee>(e)));
                })
                .ConstructUsing(x => new Core.Models.Employee(x.Id, x.Name))
                .As<Core.Models.Employee>();
            Mapper
                .CreateMap<Data.Models.Employee, Core.IEmployee>()
                .ConstructUsing(x => new Core.Models.Employee(x.Id, x.Name))
                .As<Core.Models.Employee>();
            Mapper
                .CreateMap<Data.Models.Employee, Core.Models.Employee>()
                .ForMember(d => d.Employees, d => d.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.Employees != null)
                        s.Employees.Each(e => d.GainEmployee(Mapper.Map<Core.IEmployee>(e)));
                });

            Mapper
                .CreateMap<Core.IManager, Data.Models.Employee>()
                .ForMember(d => d.Employees, d => d.MapFrom(s => s.Employees));
            Mapper
                .CreateMap<Core.IEmployee, Data.Models.Employee>()
                .Include<Core.Models.Employee, Data.Models.Employee>();
            Mapper
                .CreateMap<Core.Models.Employee, Data.Models.Employee>()
                .ForMember(d => d.Employees, d => d.MapFrom(s => s.Employees));
        }
    }
}
