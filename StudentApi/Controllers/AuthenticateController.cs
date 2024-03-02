using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentApi.Models;
using StudentApi.Repository.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi_N_Tier.Controllers
{    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        StudentDbContext _context;
        IConfiguration _config;
        public AuthenticateController(IConfiguration config, StudentDbContext context)
        {
            _config = config;
            _context = context;

        }
        [HttpPost]
        public IActionResult Login(User user1)
        {
            IActionResult response = Unauthorized();
            User user = Authenticate(user1);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;



        }

        private User Authenticate(User user)
        {
            User obj = _context.UserList.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);
                 return obj;
        }

        string GetRoleName (int roleId)
        {
           string roleName = (from x in _context.Roles
                              where x.RoleId == roleId
                              select x.RoleName).FirstOrDefault();
            return roleName;
        }
        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            string roleName = GetRoleName(user.RoleId);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid,user.Id.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(type:"DateOnly", DateTime.Now.ToString())
            };
            
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

