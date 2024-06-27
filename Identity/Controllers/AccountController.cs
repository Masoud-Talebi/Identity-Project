using Identity.Models;
using Identity.Tools;
using Identity.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Text;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSendr _emailsendr;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(UserManager<ApplicationUser> userManager , IViewRenderService viewRenderService,IEmailSendr emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailsendr = emailSender;
            _viewRenderService = viewRenderService;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            ViewBag.IsSent = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            ViewBag.IsSent = false;
            if (!ModelState.IsValid)
                return View();
            var res = await _userManager.CreateAsync(new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.Phone
            }, model.Password);
            if (!res.Succeeded)
            {
                foreach (var item in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View();
            }
            var user = await _userManager.FindByNameAsync(model.Username);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callbackUrl = Url.ActionLink("ConfirmEmail", "Account", new { UserId = user.Id, Token = token }, Request.Scheme);
            var body =await _viewRenderService.RenderToStringAsync("_EmailConfirm" , callbackUrl);
            await _emailsendr.SendEmail(user.Email, "تایید حساب کاربری", body);
            ViewBag.IsSent = true;
            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string UserId ,string token)
        {
            if (UserId == null || token == null) return BadRequest();
            var user =await _userManager.FindByIdAsync(UserId);
            if (user == null) return BadRequest();
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var res =await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.IsConfirm = res.Succeeded? true: false;
            return View();
        }
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model ,string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد.");
                return View(model);
            }
            var res = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (res.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (res.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa");
            }
            else if (res.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کابری شما قفل شده است");
                return View(model);
            }
            else if (res.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "حساب کابری خود را تایید کنید");
                return View(model);
            }
            ModelState.AddModelError(string.Empty, "ورود ناموفق لطفا گزرواژه و یوزر نیم خود را چک کنید.");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult FrogetPassword()
        {
            ViewBag.IsSent = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FrogetPassword(FrogetPasswordVM model)
        {
            ViewBag.IsSent = false;
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری یا این ایمیل وجود ندارد");
                return View(model);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callbackUrl = Url.ActionLink("ResetPassword", "Account", new { Email =user.Email, Token = token }, Request.Scheme);
            var body = await _viewRenderService.RenderToStringAsync("_PasswordReset", callbackUrl);
            await _emailsendr.SendEmail(user.Email, "بازگشایی کلمه ی عبور", body);
            ViewBag.IsSent = true;
            return View();
        }
        public IActionResult ResetPassword(string Email,string Token)
        {
            if(Email == null || Token == null) { return BadRequest(); };
            ResetPasswordVM vm = new()
            {
                Email = Email,
                token = Token
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری یا این ایمیل وجود ندارد");
                return View(model);
            }
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.token));
            var res = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!res.Succeeded)
            {  
                foreach (var item in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                    return View();
                }
            }
            return RedirectToAction("Login", "Account");


        }
        #region Remote Validation
        [HttpPost]
        public async Task<IActionResult> AnyUserName(string userName)
        {
            bool any =await _userManager.Users.AnyAsync(p=> p.UserName == userName);
            if (!any)
                return Json(true);
            return Json("نام کاربری مورد نظر تکراری است");
        }
        [HttpPost]
        public async Task<IActionResult> AnyEmail(string Email)
        {
            bool any = await _userManager.Users.AnyAsync(p => p.Email == Email);
            if (!any)
                return Json(true);
            return Json("ایمیل مورد نظر تکراری است");
        }
        #endregion


    }
}
