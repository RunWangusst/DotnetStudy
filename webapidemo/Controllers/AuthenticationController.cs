using Microsoft.AspNetCore.Authorization;
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

        [HttpPost, Route("/requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {

            if (request == null || request.userName == null && request.password == null)
            {
                return BadRequest("Invalid Request.");
            }

            // 生成 token 和 refreshtoken
            var token = GenUserToken(request.userName, "testUser");

            var refreshToken = GenUserToken(request.userName, "testUser");

            return Ok(new[] { token, refreshToken });
        }

        [HttpPost, Route("/refreshToken")]
        [Authorize]
        public ActionResult RefreshToken([FromBody] RefreshTokenDTO request)
        {
            if (request == null)
                return BadRequest("invalid request.");
            if (request.Token == null && request.RefreshToken == null)
            {
                return BadRequest("Invalid request");
            }

            // 这里是验证token的代码
            var handler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal claim = handler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenParameter.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                }, out SecurityToken securityToken);

                var username = claim.Identity.Name;

                // 这里是生成 token 的代码
                var token = GenUserToken(username, "testUser");
                var refreshToken = GenUserToken(username, "wr");
                return Ok(new[] { token, refreshToken });
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid request {ex.Message}");
            }
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
