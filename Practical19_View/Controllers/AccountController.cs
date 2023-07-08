using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practical19_View.Models;
using Practical19_View.ViewModels;
using System.Net;
using System.Net.Http.Headers;

namespace Practical19_View.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public AccountController(HttpClient httpClient, IMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("FirstName, LastName, Email, MobileNumber, Password, ConfirmPassword")]   RegisterViewModel model)
    {
        if(ModelState.IsValid)
        {
            var dataModel = _mapper.Map<RegisterModel>(model);
            var jsonData = JsonConvert.SerializeObject(dataModel);

            var response = await _httpClient.PostAsync("https://localhost:7205/api/auth/signup", new StringContent(jsonData, mediaType: MediaTypeWithQualityHeaderValue.Parse("application/json")));
            
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                return BadRequest(response);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([Bind("Email, Password, RememberMe")] LoginViewModel model)
    {
        if(ModelState.IsValid)
        {
            var dataModel = _mapper.Map<LoginModel>(model);
            var jsonData = JsonConvert.SerializeObject(dataModel);

            var response = await _httpClient.PostAsync("https://localhost:7205/api/auth/signin", new StringContent(jsonData, mediaType: MediaTypeWithQualityHeaderValue.Parse("application/json")));
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return View("Temp");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password.");
            }
        }

        return View(model);
    }
}