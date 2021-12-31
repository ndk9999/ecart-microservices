namespace Mango.Services.Identity.Models
{
    public sealed class UserRoles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static string[] GetAll()
        {
            return new[] { Admin, Customer };
        }
    }
}
