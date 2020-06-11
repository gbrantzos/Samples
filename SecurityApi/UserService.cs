using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SecurityApi
{
    public class UserService : IUserService
    {
        private static List<User> Users = new List<User>
        {
            new User
            {
                Id = 1,
                UserName = "g.brantzos",
                Password = "testMe!",
                DisplayName = "Giorgos Brantzos"
            }
        };

        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            var user = Users.SingleOrDefault(x => x.UserName == request.UserName && x.Password == request.Password);
            if (user == null)
                return null;

            var token = GenerateToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll() => Users;

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("giorgioKey1234%^");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Role, "author")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
