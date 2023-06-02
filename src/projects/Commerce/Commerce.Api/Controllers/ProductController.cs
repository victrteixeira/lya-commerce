using Commerce.Core.Entities;
using Commerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repo;

    public ProductController(IProductRepository repo) => _repo = repo;

    [HttpPost]
    [Authorize]
    [Route("add")]
    public async Task<IActionResult> Add([FromBody] Product product)
    {
        var response = await _repo.AddAsync(product);
        return Ok(response);
    }
}