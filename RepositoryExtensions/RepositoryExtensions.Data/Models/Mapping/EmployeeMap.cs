using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryExtensions.Data.Models.Mapping
{
    public class EmployeeMap :EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            HasKey(x => x.Id);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(x => x.ManagerId)
                .IsOptional();

            ToTable("Employee");
            Property(x => x.Id).HasColumnName("Id");
            Property(x => x.Name).HasColumnName("Name");
            Property(x => x.ManagerId).HasColumnName("ManagerId");

            HasOptional(x => x.Manager).WithMany(x => x.Employees);
        }
    }
}
