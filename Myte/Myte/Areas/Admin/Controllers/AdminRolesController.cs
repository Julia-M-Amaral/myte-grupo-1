﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Myte.Areas.Admin.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;

namespace Myte.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminRolesController : Controller
{
    private RoleManager<IdentityRole> roleManager;
    private UserManager<IdentityUser> userManager;

    public AdminRolesController(RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    public ViewResult Index() => View(roleManager.Roles);

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create([Required] string name)
    {
        if (ModelState.IsValid)
        {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Erros(result);

            }
        }
        return View(name);

    }

    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        IdentityRole role = await roleManager.FindByIdAsync(id);

        List<IdentityUser> members = new List<IdentityUser>();
        List<IdentityUser> nonMembres = new List<IdentityUser>();

        foreach (IdentityUser user in userManager.Users)
        {
            var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembres;
            list.Add(user);
        }
        return View(new RoleEdit
        {
            Role = role,
            Members = members,
            NonMembers = nonMembres
        });
    }


    [HttpPost]
    public async Task<IActionResult> Update(RoleModification model)
    {
        IdentityResult result;
        if (ModelState.IsValid)
        {
            foreach (string userId in model.AddIds ?? new string[] {})
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Erros(result);
                }
            }
            foreach (string userId in model.DeleteIds ?? new string[] {})
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Erros(result);
                }
            }
        }

        if (ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return await Update(model.RoleId);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        IdentityRole role = await roleManager.FindByIdAsync(id);
        if (role == null)
        {
            ModelState.AddModelError("", "Role não encontrada");
            return View("Index", roleManager.Roles);
        }
        return View(role);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if ( role != null)
        {
            IdentityResult result = await roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Erros(result);
            }

        }
        else
        {
            ModelState.AddModelError("", "Role não encontrada");
        }
        return View("Index", roleManager.Roles);
    }

    private void Erros(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}