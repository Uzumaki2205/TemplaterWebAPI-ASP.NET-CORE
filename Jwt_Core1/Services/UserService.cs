using System.Linq;
using Jwt_Core1.Models.Entities;

namespace Jwt_Core1.Services
{
    public class UserService
    {
        public TblUser Authenticate(string username, string password)
        {
            var user = new ApitemplatereportContext().TblUsers
                .Where(u => u.Username == username && u.Password == password).FirstOrDefault();

            // return null if user not found
            if (user == null)
                return null;

            return user;
        }
    }
}