using ZhmApi.Data;
using ZhmApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ZhmApi.Services
{
  public interface ITokenService
  {
    Task<Token> CreateTokenAsync(int userId, TokenType type, TimeSpan expiry);
    Task<Token?> GetValidTokenAsync(string tokenValue, TokenType type);
    Task<bool> ValidateTokenAsync(string tokenValue, TokenType type, int userId);
    Task RevokeTokenAsync(string tokenValue, TokenType type);
    Task RevokeAllUserTokensAsync(int userId, TokenType type);
    Task<bool> UseTokenAsync(string tokenValue, TokenType type);
    Task CleanupExpiredTokensAsync();
  }

  public class TokenService : ITokenService
  {
    private readonly ApiContext _context;

    public TokenService(ApiContext context)
    {
      _context = context;
    }

    public async Task<Token> CreateTokenAsync(int userId, TokenType type, TimeSpan expiry)
    {
      var token = new Token
      {
        Value = GenerateTokenValue(),
        Type = type,
        UserId = userId,
        ExpiresAt = DateTime.UtcNow.Add(expiry)
      };

      _context.Tokens.Add(token);
      await _context.SaveChangesAsync();

      return token;
    }

    public async Task<Token?> GetValidTokenAsync(string tokenValue, TokenType type)
    {
      return await _context.Tokens
          .Include(t => t.User)
          .FirstOrDefaultAsync(t =>
              t.Value == tokenValue &&
              t.Type == type &&
              !t.IsRevoked &&
              t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> ValidateTokenAsync(string tokenValue, TokenType type, int userId)
    {
      var token = await GetValidTokenAsync(tokenValue, type);
      return token != null && token.UserId == userId;
    }

    public async Task RevokeTokenAsync(string tokenValue, TokenType type)
    {
      var token = await _context.Tokens
          .FirstOrDefaultAsync(t => t.Value == tokenValue && t.Type == type);

      if (token != null)
      {
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
      }
    }

    public async Task RevokeAllUserTokensAsync(int userId, TokenType type)
    {
      var tokens = await _context.Tokens
          .Where(t => t.UserId == userId && t.Type == type && !t.IsRevoked)
          .ToListAsync();

      foreach (var token in tokens)
      {
        token.IsRevoked = true;
      }

      if (tokens.Any())
      {
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> UseTokenAsync(string tokenValue, TokenType type)
    {
      var token = await _context.Tokens
          .FirstOrDefaultAsync(t =>
              t.Value == tokenValue &&
              t.Type == type &&
              !t.IsRevoked &&
              t.ExpiresAt > DateTime.UtcNow);

      if (token == null)
        return false;

      token.IsRevoked = true; // Mark as revoked instead of used
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task CleanupExpiredTokensAsync()
    {
      var expiredTokens = await _context.Tokens
          .Where(t => t.ExpiresAt <= DateTime.UtcNow)
          .ToListAsync();

      _context.Tokens.RemoveRange(expiredTokens);
      await _context.SaveChangesAsync();
    }

    private static string GenerateTokenValue()
    {
      var randomBytes = new byte[32];
      using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
      rng.GetBytes(randomBytes);
      return Convert.ToBase64String(randomBytes);
    }
  }
}