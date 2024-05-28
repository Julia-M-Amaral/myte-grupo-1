using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Myte.Models;

namespace Myte.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Myte.Models.WBS> WBS { get; set; } = default!;
        public DbSet<Myte.Models.Registro> Registro { get; set; } = default!;
        public DbSet<Myte.Models.Funcionario> Funcionario { get; set; } = default!;
        public DbSet<Myte.Models.Departamento> Departamento { get; set; } = default!;
    }
}
