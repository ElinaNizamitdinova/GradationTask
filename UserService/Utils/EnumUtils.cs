using PostGreDbContext.Enums;

namespace UserService.Utils
{
    public static class EnumUtils
    {
        public static int GetIntFromEnumName(string name)
        {
            if (Enum.IsDefined(typeof(UserRoleTypeEnum), name))
                return (int)Enum.Parse(typeof(UserRoleTypeEnum), name);
            throw new Exception("Invalid enum cast");
        }
    }
}
