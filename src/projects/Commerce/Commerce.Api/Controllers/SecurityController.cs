using Commerce.Api.Utils;
using Commerce.Security.DTOs;
using Commerce.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ISecurityService _service;

    public SecurityController(ISecurityService service) => _service = service;

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] CreateUser command)
    {
        var serviceResponse = await _service.RegisterAsync(command);
        var apiResponse = ApiResponse<ReadUser>.Success(serviceResponse, "Usuário cadastrado com sucesso.");

        return Ok(apiResponse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("register/admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] CreateUser command)
    {
        var serviceResponse = await _service.RegisterAdminAsync(command);
        var apiResponse = ApiResponse<ReadUser>.Success(serviceResponse, "Usuário cadastrado com sucesso.");

        return Ok(apiResponse);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser command)
    {
        var response = await _service.LoginAsync(command);
        var apiResponse =
            ApiResponse<string>.Success(response, "Login bem-sucedido.");
        return Ok(apiResponse);
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin,Developer")]
    [Route("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser command)
    {
        await _service.ChangePasswordAsync(command);
        var apiResponse =
            ApiResponse<string>.Success(null, "Senha atualizada com sucesso.");
        return Ok(apiResponse);
    }
}