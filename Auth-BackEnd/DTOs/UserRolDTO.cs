using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.DTOs
{
    public class UserRolDTO
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Rol is required")]
        public string Rol { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public bool Status { get; set; }

    }
}
