using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Practical19_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Practical19_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,  
        IMapper mapper,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _mapper = mapper;
        _config = configuration;
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
        IActionResult response = Unauthorized();

        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var tokenString = GenerateJSONWebToken(model);
            response = Ok(new { Token = tokenString, Message = "Success" });
        }

        return response;

        //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        //if (result.Succeeded)
        //{

        //    return Ok();
        //}   

        //return BadRequest("Email or password incorrect.");
    }

    private string GenerateJSONWebToken(LoginModel userInfo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Issuer"],
          null,
          expires: DateTime.Now.AddMinutes(120),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}