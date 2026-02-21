using API_final.DTOs;
using API_final.Entities;
using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace API_final.Services.Implementatios
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;    

        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            // 1. Verificamos que el mail no exista
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("El email ya está registrado.");
            }

            // 2. HASHEO DE CONTRASEÑA (REQUERIMIENTO EXTRA)
            // BCrypt genera el "Salt" automáticamente y lo incluye en el hash resultante.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Mapeamos el DTO a la Entidad
            var user = new User
            {
                RestaurantName = dto.RestaurantName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Address = dto.Address
            };

            await _userRepository.AddAsync(user);

            return "Restaurante registrado con éxito.";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Name, user.RestaurantName)
           };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
