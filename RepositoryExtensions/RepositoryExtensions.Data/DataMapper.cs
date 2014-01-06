using System.Linq;
using AutoMapper;

namespace RepositoryExtensions.Data
{
    public class DataMapper
    {
        private const int DEPTH = 1;

        public void Configure()
        {
            Mapper
                .CreateMap<Data.Models.Employee, Core.IManager>()
                .ForMember(d => d.Employees, d => d.Ignore())
                .AfterMap((s, d) => {
                    if (s.Employees != null)
                        s.Employees.Each(e => d.GainEmployee(Mapper.Map<Core.IEmployee>(e)));
                })
                .As<Core.Model.Employee>();
            Mapper
                .CreateMap<Data.Models.Employee, Core.IEmployee>()
                .As<Core.Model.Employee>();
            Mapper
                .CreateMap<Data.Models.Employee, Core.Model.Employee>()
                .ForMember(d => d.Employees, d => d.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.Employees != null)
                        s.Employees.Each(e => d.GainEmployee(Mapper.Map<Core.IEmployee>(e)));
                });
        }
    }
}
