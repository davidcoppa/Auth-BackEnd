using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.Model
{
    public class UserToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
