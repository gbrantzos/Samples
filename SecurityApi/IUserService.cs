using System.Collections.Generic;

namespace SecurityApi
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request);
        IEnumerable<User> GetAll();
    }
}