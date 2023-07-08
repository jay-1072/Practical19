using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practical19_API.Models;

namespace Practical19_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> RegisterUser(RegisterModel model)
    {
        var newUser = _mapper.Map<ApplicationUser>(model);

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if(result.Succeeded)
        {
            return Created(string.Empty, string.Empty); 
        }
        return Problem(result.Errors.First().Description, null, 500);
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        
        if(result.Succeeded)
        {
            return Ok();
        }   

        return BadRequest("Email or password incorrect.");
    }
}