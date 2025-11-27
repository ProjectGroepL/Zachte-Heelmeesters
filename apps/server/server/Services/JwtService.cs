using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ZhmApi.Services
{
  public interface IJwtService
  {
    string GenerateToken(int userId);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
  }

  public class JwtService : IJwtService
  {
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
      // Read from environment variables first, then fall back to configuration
      _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? configuration["JWT:Key"]!;
      _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JWT:Issuer"]!;
      _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JWT:Audience"]!;
      _expirationMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? configuration["JWT:ExpirationMinutes"] ?? "60");
    }

    public string GenerateToken(int userId)
    {
      var claims = new[]
      {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.UtcNow.AddMinutes(_expirationMinutes);

      var token = new JwtSecurityToken(
          issuer: _issuer,
          audience: _audience,
          claims: claims,
          expires: expires,
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
      var randomBytes = new byte[32];
      using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
      rng.GetBytes(randomBytes);
      return Convert.ToBase64String(randomBytes);
    }

    public bool ValidateToken(string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = _issuer,
          ValidateAudience = true,
          ValidAudience = _audience,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        return true;
      }
      catch
      {
        return false;
      }
    }

    public int? GetUserIdFromToken(string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = _issuer,
          ValidateAudience = true,
          ValidAudience = _audience,
          ValidateLifetime = false, // Don't validate lifetime for this method
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
          return userId;
        }
        return null;
      }
      catch
      {
        return null;
      }
    }
  }
}
