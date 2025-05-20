using Domain.DTOs.Admin;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class AdminRController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AdminRController(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
    {
        var user = new AppUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.Phone,First_name=model.FirstName,Last_name=model.LastName };
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            return BadRequest("The 'Admin' role does not exist.");
        }
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);

        return Ok(new { Message = "Admin registered successfully" });
    }
}
