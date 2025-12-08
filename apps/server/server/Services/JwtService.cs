using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZhmApi.Models;
using Microsoft.AspNetCore.Identity;
using ZhmApi.Data;

namespace ZhmApi.Services
{
  public interface IJwtService
  {
    string GenerateToken(int userId);
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
  }

  public class JwtService : IJwtService
  {
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    private readonly ApiContext _db;

    public JwtService(IConfiguration configuration, ApiContext db)
    {
      _db = db;

      _key = Environment.GetEnvironmentVariable("JWT_KEY")
             ?? configuration["JWT:Key"]!;
      _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? configuration["JWT:Issuer"]!;
      _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                  ?? configuration["JWT:Audience"]!;
      _expirationMinutes = int.Parse(
          Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES")
          ?? configuration["JWT:ExpirationMinutes"]
          ?? "60"
      );
    }

    public string GenerateToken(int userId)
    {
      var user = _db.Users.FirstOrDefault(u => u.Id == userId);
      if (user == null)
        throw new InvalidOperationException($"User with id {userId} not found when generating JWT");

      // --- Claims toevoegen ---
      var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),   // Nodig voor .NET auth
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Nodig voor consistentie
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

      // --- Roles toevoegen ---
      var roles = _db
          .Set<IdentityUserRole<int>>()
          .Where(ur => ur.UserId == userId)
          .Join(_db.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
          .ToList();

      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role ?? ""));
      }

      // --- Token bouwen ---
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _issuer,
          audience: _audience,
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
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

        }, out _);

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
          ValidateLifetime = false, // We only want info, not validation logic
          ClockSkew = TimeSpan.Zero

        }, out _);

        // Probeer eerst SUB, anders NameIdentifier
        var userIdClaim =
            principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userIdClaim, out int userId))
          return userId;

        return null;
      }
      catch
      {
        return null;
      }
    }
  }
}
