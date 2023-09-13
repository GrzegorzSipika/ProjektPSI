using System.Security.Claims;
using LoginProject.Common.Entities;
using LoginProject.Common.Enums;
using LoginProject.Common.ViewModels;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LoginProject.Services;

public class AuthenticationUserService : IAuthenticationUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserActionInfoRepository _userActionInfoRepository;
    private readonly IUserPasswordHistoryRepository _userPasswordHistoryRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IMailSenderService _mailSenderService;

    public AuthenticationUserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IUserActionInfoRepository userActionInfoRepository,
        IUserPasswordHistoryRepository userPasswordHistoryRepository,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMailSenderService mailSenderService
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _userActionInfoRepository = userActionInfoRepository;
        _userPasswordHistoryRepository = userPasswordHistoryRepository;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _mailSenderService = mailSenderService;
    }

    public async Task<ResponseInforamation> Login(UserLoginVm userLoginVm)
    {
        var status = new ResponseInforamation();

        if (!IsIpAllowed(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()))
        {
            status.IsSuccess = false;
            status.Message = "Użytkownik z tego adresu IP nie ma dostępu do aplikacji";
            return status;
        }

        var user = await _userManager.FindByNameAsync(userLoginVm.Username);
        if (user == null)
        {
            status.IsSuccess = false;
            status.Message = "Nie znaleziono użytkownika o podanym loginie";
            return status;
        }

        if (user.IsBlocked)
        {
            status.IsSuccess = false;
            status.Message = $"Użytkownik zablokowany do {user.BlockedTo}";
            return status;
        }

        if (!await _userManager.CheckPasswordAsync(user, userLoginVm.Password))
        {
            status.IsSuccess = false;
            status.Message = "Niepoprawne hasło";
            await _userActionInfoRepository.AddUserActionInfo(new UserActionInfo
            {
                UserId = user.Id,
                ActionType = ActionType.Login,
                IsSuccess = false
            });
            var lastThreeLoginActionWasFailed = await _userActionInfoRepository.IsLastThreeLoginActionWasFailed(user);
            if (lastThreeLoginActionWasFailed)
            {
                user.BlockedTo = DateTime.Now.AddMinutes(1);
                await _userManager.UpdateAsync(user);
            }
            return status;
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, userLoginVm.Password, false, true);
        if (signInResult.Succeeded)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            status.IsSuccess = true;
            status.Message = "Logowanie powiodło się";
            await _userActionInfoRepository.AddUserActionInfo(new UserActionInfo
            {
                UserId = user.Id,
                ActionType = ActionType.Login,
                IsSuccess = true
            });
        }
        else if (signInResult.IsLockedOut)
        {
            status.IsSuccess = false;
            status.Message = "Konto zablokowane";
        }
        else
        {
            status.IsSuccess = false;
            status.Message = "Problem z logowaniem";
        }

        return status;
    }

    public async Task<ResponseInforamation> ChangePassword(ChangePasswordVm changePasswordVm)
    {
        var status = new ResponseInforamation();

        var user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
        
        if (user == null)
        {
            status.Message = "Użytkonik nie istnieje";
            status.IsSuccess = false;
            return status;
        }

        if (await _userPasswordHistoryRepository.IsPasswordWasChangedLessThenXDays(user, 1))
        {
            status.Message = "Hasło było zmieniane w ciągu ostatnich 24 godzin";
            status.IsSuccess = false;
            return status;
        }

        if (await _userPasswordHistoryRepository.IsPasswordWasUsedInLastTwentyPasswords(user,changePasswordVm.NewPassword))
        {
            status.Message = "Hasło było używane w ciągu ostatnich 20 zmian hasła";
            status.IsSuccess = false;
            return status;
        }

        var result = await _userManager.ChangePasswordAsync(user, changePasswordVm.CurrentPassword, changePasswordVm.NewPassword);
        if (result.Succeeded)
        {
            await _userPasswordHistoryRepository.AddPasswordToHistory(user.Id, changePasswordVm.NewPassword);
            status.Message = "Hasło zostało zmienione";
            status.IsSuccess = true;
        }
        else
        {
            status.Message = "Nie udało się zmienić hasła";
            status.IsSuccess = false;
        }

        return status;
    }

    public async Task<ResponseInforamation> Registration(UserRegisterVm userRegisterVm)
    {
        var status = new ResponseInforamation();
        var userExists = await _userManager.FindByNameAsync(userRegisterVm.Username);
        if (userExists != null)
        {
            status.IsSuccess = false;
            status.Message = "Użytkownik o podanym loginie już istnieje";
            return status;
        }

        ApplicationUser user = new ApplicationUser()
        {
            Name = userRegisterVm.Username,
            Email = userRegisterVm.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userRegisterVm.Username,
            FirstName = userRegisterVm.FirstName,
            LastName = userRegisterVm.LastName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, userRegisterVm.Password);

        if (!result.Succeeded)
        {
            status.IsSuccess = false;
            status.Message = "Nie udało się zarejestrować użytkownika";
            return status;
        }

        if (!await _roleManager.RoleExistsAsync(userRegisterVm.Role))
            await _roleManager.CreateAsync(new IdentityRole(userRegisterVm.Role));


        if (await _roleManager.RoleExistsAsync(userRegisterVm.Role))
        {
            await _userManager.AddToRoleAsync(user, userRegisterVm.Role);
        }

        await _userPasswordHistoryRepository.AddPasswordToHistory(user.Id, userRegisterVm.Password);

        status.IsSuccess = true;
        status.Message = "Użytkownik został zarejestrowany";

        return status;
    }

    public async Task Logout()
    {
        var user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
        
        await _userActionInfoRepository.AddUserActionInfo(new UserActionInfo
        {
            UserId = user.Id,
            ActionType = ActionType.Logout,
            IsSuccess = true
        });
        
        await _signInManager.SignOutAsync();
    }

    public async Task<ResponseInforamation> RememberPasswordSendMail(RememberPasswordVm model)
    {
        var status = new ResponseInforamation();
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            status.IsSuccess = false;
            status.Message = "Nie znaleziono użytkownika o podanym adresie email";
            return status;
        }
        
        string newPassword = Guid.NewGuid().ToString();
        newPassword += ("QW@!%^");
        
        await _mailSenderService.SendMail(
            model.Email,
            "Przypomnienie hasła",
            $"Twoje hasło to: {newPassword}");
        
        var result = await _userManager.RemovePasswordAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddPasswordAsync(user, newPassword);
            if (result.Succeeded)
            {
                status.IsSuccess = true;
                status.Message = "Hasło zostało wysłane na podany adres email";
                await _userPasswordHistoryRepository.AddPasswordToHistory(user.Id, newPassword);
            }
            else
            {
                status.IsSuccess = false;
                status.Message = "Nie udało się wysłać hasła na podany adres email";
            }
        }
        else
        {
            status.IsSuccess = false;
            status.Message = "Nie udało się wysłać hasła na podany adres email";
        }

        return status;
    }

    private bool IsIpAllowed(string ip)
    {
        var allowedIps = _configuration.GetSection("AllowedIps").Get<List<string>>();
        return allowedIps.Contains(ip);
    }
}