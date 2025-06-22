using LaundryCleaning.Service.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class User : EntityBase
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<Identity> Identities { get; set; }
    }
}