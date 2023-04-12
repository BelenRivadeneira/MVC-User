using MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC.Data
{
    public class UserContext : DbContext
    {
        //Constructor de la clase
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        //Propiedad DbSet para acceder a la tabla de usuarios en la base de datos
        public DbSet<User> Users { get; set; }

        //Configuración de Fluent API para la tabla de usuarios
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}