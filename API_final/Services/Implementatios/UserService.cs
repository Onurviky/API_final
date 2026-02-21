using API_final.DTOs;
using API_final.Entities;
using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;
using BCrypt.Net;

namespace API_final.Services.Implementatios
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            // 1. Buscamos al usuario
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            // 2. VERIFICACIÓN DEL HASH (REQUERIMIENTO EXTRA)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            // ACÁ LUEGO GENERAREMOS EL TOKEN JWT. Por ahora devolvemos un string.
            return "Login exitoso. (Pronto devolveremos un JWT acá)";
        }
    }
}
