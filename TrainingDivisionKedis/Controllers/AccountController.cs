using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.User;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.Account;

namespace TrainingDivisionKedis.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        //private readonly SignInManager<AppUser> _signInManager;



        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Register(string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        UserDto userDto = new UserDto
        //        {
        //            Email = model.Email,
        //            Password = model.Password,
        //            Role = RoleEnum.Client.ToString()
        //        };

        //        OperationDetails<UserDto> operationDetails = await _userService.CreateAsync(userDto);
        //        if (operationDetails.IsSuccess)
        //            return RedirectToAction("Login");
        //        else
        //        {
        //            AddErrors(operationDetails);
        //            return View(model);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var userDto = new UserDto(model.Login, model.Password);
                var result = await _userService.AuthenticateAsync(userDto);
                if(result.Succedeed)
                {
                    await Authenticate(result.Entity);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                //await _signInManager.SignOutAsync();
                //var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                //if (result.Succeeded)
                //    return RedirectToLocal(returnUrl);
                //else
                //    ModelState.AddModelError("", "Неверный логин или пароль.");

            }
            return View(model);
        }

        protected virtual async Task Authenticate(UserDto user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimTypes.Surname, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public virtual async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                BadRequest();
            request.Id = id;
            var result = await _userService.ChangePasswordAsync(request);
            if (result.Succedeed)
            {
                await Logout();
                return RedirectToAction("Login");
            }
            else
            {
                return View().WithDanger("Ошибка!", result.Error.Message);
            }
        }

        [Authorize]
        public IActionResult ChangeLogin()
        {
            var model = new ChangeLoginViewModel() { OldLogin = User.Identity.Name };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeLogin(ChangeUserLoginRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                BadRequest();

            request.Id = id;
            var result = await _userService.ChangeLoginAsync(request);
            if (result.Succedeed)
            {
                await Logout();
                return RedirectToAction("Login");
            }
            else
            {
                return View(request).WithDanger("Ошибка!", result.Error.Message);
            }
        }

        #region Helpers        

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url != null && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

    }
}