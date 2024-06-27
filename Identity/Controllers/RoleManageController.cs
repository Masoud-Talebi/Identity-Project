using Identity.Models;
using Identity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleManageController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleManageController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var model =await _roleManager.Roles.Select(p=> new RoleVM()
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
            return View(model);
        }
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string Name)
        {
            var res =await _roleManager.CreateAsync(new ApplicationRole() { Name = Name });
            if (res.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            await _roleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            RoleVM roleVM = new RoleVM()
            {
                Id = role.Id,
                Name = role.Name
            };
            if (role == null)
                return NotFound();

            return View(roleVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            role.Name = name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(role);
        }
    }
}
