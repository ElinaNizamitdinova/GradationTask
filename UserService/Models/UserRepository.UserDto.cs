using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;
using UserService.Utils;

namespace UserService.Models
{
    public partial class UserRepository
    {
        public class UserDto
        {
            public Guid? Guid { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string UserRole { get; set; }
            [MinLength(8)]
            public string Password { get; set; }
            public int UserRoleId => EnumUtils.GetIntFromEnumName(UserRole);
        }
    }
}
