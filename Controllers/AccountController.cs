﻿using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Account;

namespace WebAdvert.Web.Controllers
{
  public class AccountController : Controller
  {
    private readonly SignInManager<CognitoUser> _signInManager;
    private readonly UserManager<CognitoUser> _userManager;
    private readonly CognitoUserPool _pool;

    public AccountController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager,
                             CognitoUserPool pool)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _pool = pool;
    }

    public async Task<IActionResult> Signup()
    {
      var model = new SignupModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupModel model)
    {
      if (ModelState.IsValid)
      {
        var user = _pool.GetUser(model.Email);
        if (user != null && user.UserID != null)
        {
          ModelState.AddModelError("UserExists", "User with this email already exists");
          return View(model);
        }

        user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
        var createdUser = await _userManager.CreateAsync(user, model.Password);

        if (createdUser.Succeeded)
          RedirectToAction("Confirm");
      }

      return View(model);
    }

    public async Task<IActionResult> Confirm()
    {
      var model = new ConfirmModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(ConfirmModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
          ModelState.AddModelError("NotFound", "A user with the given email address was not found");
          return View(model);
        }

        var result = await ((CognitoUserManager<CognitoUser>)_userManager).ConfirmSignUpAsync(user, model.Code, true);

        if (result.Succeeded)
          return RedirectToAction("Index", "Home");
        else
        {
          foreach (var item in result.Errors)
            ModelState.AddModelError(item.Code, item.Description);

          return View(model);
        }
      }

      return View(model);
    }

    public async Task<IActionResult> Login()
    {
      var model = new LoginModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
          return RedirectToAction("Index", "Home");
        else
          ModelState.AddModelError("LoginError", "Email and password do not match");
      }

      return View(model);
    }
  }
}
