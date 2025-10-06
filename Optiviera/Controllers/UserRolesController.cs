using Microsoft.AspNetCore.Mvc;
using Optiviera.Services.Interfaces;
using Optiviera.Models.ViewModels;
using Optiviera.Models;
using Optiviera.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Optiviera.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly ITRolesService _rolesService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<WaveUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(ITRolesService rolesService, ApplicationDbContext context, UserManager<WaveUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _rolesService = rolesService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();
            List<WaveUser> users = await _context.Users.ToListAsync();

            foreach(WaveUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.User = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAync(), "Name", "Name", selected);
                model.Add(viewModel);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            WaveUser waveUser = (await _context.Users.ToListAsync()).FirstOrDefault(u => u.Id == member.User.Id);
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(waveUser);
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if(await _rolesService.RemoveUserFromRolesAsync(waveUser, roles))
                {
                    await _rolesService.AddUserToRoleAsync(waveUser, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles = new SelectList(await _rolesService.GetRolesAync(), "Name", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new WaveUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(ManageUserRoles));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.Roles = new SelectList(await _rolesService.GetRolesAync(), "Name", "Name");
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SelectedRole = userRoles.FirstOrDefault()
            };

            ViewBag.Roles = new SelectList(await _rolesService.GetRolesAync(), "Name", "Name", model.SelectedRole);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);

                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction(nameof(ManageUserRoles));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.Roles = new SelectList(await _rolesService.GetRolesAync(), "Name", "Name", model.SelectedRole);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken hata oluştu.";
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
