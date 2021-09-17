using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Models
{
    public class UserEdit
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        // add confirm pwd
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        [Display(Name = "Password")]
        // added confirm pwd

        [DataType(DataType.Password), MinLength(4, ErrorMessage = "Minimum length is 4")]
        public string Password { get; set; }

        //add confirm pwd
        [Required(ErrorMessage = "Please confirm your password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        // added confirm pwd

        public UserEdit() { }

        public UserEdit(AppUser appUser)
        {
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }
}
