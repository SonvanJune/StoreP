using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StoreSp.Commonds;
using StoreSp.Configs;
using StoreSp.Entities;

namespace StoreSp.Services.Impl;

public class AuthServiceImpl : IAuthService
{
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public string GetFirstByToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;

        return jwtToken.Claims.First().Value;
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        Role role = user.Role!;
        var claims = new ClaimsIdentity();
        if (user.Email != null && user.Email != "")
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        }
        else
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Phone));
        }
        claims.AddClaim(new Claim(ClaimTypes.Role, role.Code));
        return claims;
    }

    public static string CreateRandomToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var claims = new ClaimsIdentity();
        if (user.Email != null && user.Email != "")
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        }
        else
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Phone));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public static string CreateRefreshToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var claims = new ClaimsIdentity();
        if (user.Email != null && user.Email != "")
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        }
        else
        {
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Phone));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddDays(5),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public static string CreateRandomNumerToken(string randomCode)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthConfig.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Name, randomCode));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(120),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public IResult GetResult(string authorization, IResult result)
    {
        string[] strings = authorization.Split(' ');
        if (this.ValidateToken(strings[1]))
        {
            return result;
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Token has expired",
                data = null
            });
        }
    }
}
