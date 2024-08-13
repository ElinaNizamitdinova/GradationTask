using PostGreDbContext.Enums;

namespace UserService.Models
{
    public partial class UserRepository
    {
        public class UserResponse
        {
            public string UserEmail { get; set; }
            public int UserRoleId { get; set; }
            public string UserRole => Enum.GetName(typeof(UserRoleTypeEnum), UserRoleId);
        }
    }
}
