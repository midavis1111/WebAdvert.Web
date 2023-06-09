﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebAdvert.Web.Models.Account
{
  public class LoginModel
  {
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
  }
}
