using System.Diagnostics;
using LoginProject.Common.Entities;
using LoginProject.Common.ViewModels;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserActionInfoRepository _userActionInfoRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, IUserActionInfoRepository userActionInfoRepository, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userActionInfoRepository = userActionInfoRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeVm();
        var userName = User.Identity.Name;
        
        if (!string.IsNullOrWhiteSpace(userName))
        {
            var user = await _userManager.FindByNameAsync(userName);
            var response = await _userActionInfoRepository.GetUserActionInfos(user.Id);
            vm.UserActionInfos = response;
        }
        
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}