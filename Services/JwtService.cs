using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagementApis.Models;

namespace TaskManagementApis.Services;

public class JwtService {
  private readonly IConfiguration _config;

  public JwtService(IConfiguration config) => _config = config;

  public string GenerateToken(Account account, Role role) {
    var claims = new[] {
        // new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, account.Username),
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim(ClaimTypes.Role, role.RoleName)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"])
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

