using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MVC.Data;
using MVC.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext _context;

        public LoginController(IConfiguration configuration, UserContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser userLogin)
        {
            // Autentica al usuario y devuelve los detalles del usuario si se autentica correctamente.
            var user = await Authenticate(userLogin);

            if (user != null)
            {
                // Crea el token de autenticación utilizando los detalles del usuario.
                var token = Generate(user);

                // Devuelve el token de autenticación.
                return Ok(token);
            }

            // Devuelve un mensaje de error si el usuario no se autentica correctamente.
            return NotFound("Usuario no encontrado");
        }

        private async Task<User> Authenticate(LoginUser userLogin)
        {
            // Busca al usuario en la base de datos por su nombre de usuario.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirstNameUser.ToLower() == userLogin.UserName.ToLower());

            // Si el usuario no existe, devuelve null.
            if (user == null)
            {
                return null;
            }

            // Si la contraseña no coincide, devuelve null.
            if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return null;
            }

            // Devuelve los detalles del usuario si se autentica correctamente.
            return user;
        }

        private string Generate(User user)
        {
            // Crea la clave de seguridad utilizando la clave secreta especificada en appsettings.json.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // Crea las credenciales para firmar el token utilizando la clave de seguridad.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crea las claims que se incluirán en el token.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstNameUser),
                new Claim(ClaimTypes.Surname, user.LastNameUser),
            };

            // Crea el token utilizando los detalles del usuario, las credenciales y la duración de validez del token.
            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials
                );

            // Devuelve el token como una cadena de caracteres.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
