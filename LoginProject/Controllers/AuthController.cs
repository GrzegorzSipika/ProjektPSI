using LoginProject.Common.Entities;
using LoginProject.Common.ViewModels;
using LoginProject.Interfaces;
using LoginProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginProject.Controllers;

public class AuthController : Controller
{
    private readonly IAuthenticationUserService _authenticationUserService;

    public AuthController(IAuthenticationUserService authenticationUserService)
    {
        _authenticationUserService = authenticationUserService;
    }

    public IActionResult Login()
    {
        if (HttpContext.User.Identity?.Name != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    public IActionResult Registration()
    {
        return View();
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registration(UserRegisterVm model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Role = "user";
        var result = await _authenticationUserService.Registration(model);
        TempData["msg"] = result.Message;
        return RedirectToAction("Registration", "Auth");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authenticationUserService.Logout();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginVm model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var result = await _authenticationUserService.Login(model);
        if (result.IsSuccess)
        {
            return RedirectToAction("Index", "Home");
        }

        TempData["msg"] = result.Message;
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authenticationUserService.ChangePassword(model);
        TempData["msg"] = result.Message;

        return RedirectToAction("ChangePassword", "Auth");
    }

    public async Task<IActionResult> RememberPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RememberPassword(RememberPasswordVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authenticationUserService.RememberPasswordSendMail(model);

        TempData["msg"] = result.Message;

        return RedirectToAction("Login", "Auth");
    }
}