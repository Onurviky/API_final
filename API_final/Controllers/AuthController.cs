using API_final.DTOs;
using API_final.Repository.Interfaces;
using API_final.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_final.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    // Inyectamos el servicio, NO el repositorio. Respetamos la arquitectura.
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous] // Cualquier invitado puede pegarle a este endpoint
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            // El ModelState.IsValid se chequea automáticamente gracias al [ApiController]
            var result = await _userService.RegisterAsync(dto);
            return Ok(new { message = result }); // Retornamos un 200 OK
        }
        catch (Exception ex)
        {
            // Si el email ya existe, el servicio tira excepción y acá devolvemos 400
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _userService.LoginAsync(dto);

            // Si todo sale bien, le mandamos el token al cliente
            return Ok(new { token = token });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Si le pifia a la clave, devolvemos un 401 explícito (Principio REST)
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
