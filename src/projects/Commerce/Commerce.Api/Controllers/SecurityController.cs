using Commerce.Api.Utils;
using Commerce.Security.DTOs;
using Commerce.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ISecurityService _service;

    public SecurityController(ISecurityService service) => _service = service;

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] CreateUser command)
    {
        var serviceResponse = await _service.RegisterAsync(command);
        var apiResponse = ApiResponse<ReadUser>.Success(serviceResponse, "Usuário cadastrado com sucesso.");

        return Ok(apiResponse);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser command)
    {
        var response = await _service.LoginAsync(command);
        var apiResponse =
            ApiResponse<ReadUser>.Success(response, $"{response.FirstName} {response.LastName}, bem-vindo!");
        return Ok(apiResponse);
    }
}