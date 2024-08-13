namespace UserService.Models
{
    public partial class UserRepository
    {
        public class AuthorizationResponse
        {
            public string Token { get; set; }
            public DateTime ValidTo { get; set; }
        }
        
    }
}
