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
        public void Configure()
        {
            // DB to Domain
            Mapper
                .CreateMap<Data.Models.Employee, RepositoryExtensions.Core.IManager>();
            Mapper
                .CreateMap<Data.Models.Employee, RepositoryExtensions.Core.IEmployee>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.IManager, RepositoryExtensions.Core.Model.Employee>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.IEmployee, RepositoryExtensions.Core.Model.Employee>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.Model.Employee, RepositoryExtensions.Core.IManager>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.Model.Employee, RepositoryExtensions.Core.IEmployee>();
            Mapper
                .CreateMap<Data.Models.Employee, RepositoryExtensions.Core.Model.Employee>();

            // Domain to DB
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.Model.Employee, Data.Models.Employee>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.IEmployee, Data.Models.Employee>();
            //Mapper
            //    .CreateMap<RepositoryExtensions.Core.IManager, Data.Models.Employee>();
        }
    }
}
