using LoginProject.Common.Entities;
using LoginProject.Common.ViewModels;

namespace LoginProject.Interfaces;

public interface IAuthenticationUserService
{
    Task<ResponseInforamation> Login(UserLoginVm userLoginVm);
    Task<ResponseInforamation> ChangePassword(ChangePasswordVm changePasswordVm);
    Task<ResponseInforamation> Registration(UserRegisterVm userRegisterVm);
    Task<ResponseInforamation> RememberPasswordSendMail(RememberPasswordVm model);
    Task Logout();
}