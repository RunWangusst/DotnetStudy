using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapidemo.DTOModels;
using webapidemo.Models;

namespace webapidemo.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private TokenParameter _tokenParameter = new TokenParameter();

        public AuthenticationController()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _tokenParameter = config.GetSection("tokenParameter").Get<TokenParameter>();

        }

        [HttpPost, Route("/requesttoken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO loginRequestDTO)
        {

            if (loginRequestDTO == null || loginRequestDTO.userName == null && loginRequestDTO.password == null)
            {
                return BadRequest("Invalid Request.");
            }

            // 生成 token 和 refreshtoken
            var token = GenUserToken(loginRequestDTO.userName, "testUser");

            var refreshToken = GenUserToken(loginRequestDTO.userName, "testUser");

            return Ok(new[] { token, refreshToken });
        }

        private string GenUserToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.Role,role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenParameter.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_tokenParameter.Issuer, null, claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenParameter.AccessExpiration),
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }
    }
}
