using Identity.Models;
using Identity.Tools;
using Identity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [Authorize]
    public class UserManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserManageController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var res = await _userManager.Users.Select(p=> new UsersVM()
            {
                Id = p.Id,
                Email = p.Email,
                FullName = p.FirstName + " " + p.LastName,
                Phone = p.PhoneNumber,
                UserName = p.UserName
            }).ToListAsync();
            return View(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) 
                return NotFound();
            var res = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound();
            EditUserVM model = new()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                UserName = user.UserName
            };
            var role = await _roleManager.Roles.Select(p => new RoleVM()
            {
                Id = p.Id,
                Name = p.Name,
            }).ToListAsync();
            ViewBag.Roles = role;
            ViewBag.UserRoles =await _userManager.GetRolesAsync(user);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM model,List<string> SelectedRoles)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();
            user.PhoneNumber = model.Phone;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var res = await _userManager.UpdateAsync(user);
            var userrole = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user,userrole);
            await _userManager.AddToRolesAsync(user, SelectedRoles);

            if (res.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("index");
            }

            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var Model = new EditUserVM()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                UserName = user.UserName
            };

            var rolesList = _roleManager.Roles
                .Select(r => new RoleVM()
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();

            ViewBag.Roles = rolesList;
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(Model);

        }

    }
}
