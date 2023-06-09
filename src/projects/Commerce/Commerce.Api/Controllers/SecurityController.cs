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

    [HttpGet]
    [AllowAnonymous]
    [Route("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("A token must be supplied for email confirmation.");
        }

        var response = await _service.ConfirmEmailAsync(token);
        var apiResponse =
            ApiResponse<bool>.Success(response, "E-mail confirmado com sucesso, você pode realizar o login agora!");
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
    [Route("password/change")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser command)
    {
        await _service.ChangePasswordAsync(command);
        var apiResponse =
            ApiResponse<string>.Success(null, "Senha atualizada com sucesso.");
        return Ok(apiResponse);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("password/request")]
    public async Task<IActionResult> ForgotPasswordRequest([FromBody] string? email)
    {
        await _service.ForgotPasswordRequest(email);
        var apiResponse =
            ApiResponse<string>.Success(null, "E-mail de recuperação de senha enviado com sucesso.");
        return Ok(apiResponse);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("forgot/recovery")]
    public async Task<IActionResult> PasswordRecovery([FromQuery] string? token, [FromBody] ResetPasswordUser command)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("A token must be supplied for password recovery.");
        }

        await _service.PasswordRecovery(token, command);
        var apiResponse = 
            ApiResponse<string>.Success(null, "Senha alterada com sucesso.");
        return Ok(apiResponse);
    }
}