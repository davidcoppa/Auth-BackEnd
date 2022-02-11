using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.Model
{
    public class UserInfo
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8,ErrorMessage ="Password min length is 8 characters")]
        public string Password { get; set; }
    }
}
