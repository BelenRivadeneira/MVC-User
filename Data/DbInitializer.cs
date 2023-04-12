using MVC.Models;
using BCrypt.Net;
using System.Linq;

namespace MVC.Data
{
    public static class DbInitializer
    {
        public static void Initialize(UserContext context)
        {
            context.Database.EnsureCreated(); // asegura la creación de la base de datos

            // Busca si ya hay usuarios en la base de datos.
            if (context.Users.Any())
            {
                return;   // La base de datos ya ha sido sembrada
            }

            // Crea una lista de usuarios por defecto
            var users = new User[]
            {
                new User{FirstNameUser="ADMIN",LastNameUser="ADMIN",Password=BCrypt.Net.BCrypt.HashPassword("admin123")} // Crea un usuario ADMIN con la contraseña "admin123"
            };

            // Agrega los usuarios a la base de datos y los guarda
            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges(); // Guarda los cambios en la base de datos
        }
    }
}