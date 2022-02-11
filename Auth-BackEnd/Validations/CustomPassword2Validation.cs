using Auth_BackEnd.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.Validations
{
    public class CustomPassword2Validation: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var newUser = (NewUserDTO)validationContext.ObjectInstance;

            if (newUser.Password.CompareTo((string)value)!=0)
            {
                return new ValidationResult("Password must match!");
            }
            return ValidationResult.Success;


        }
    }
}
