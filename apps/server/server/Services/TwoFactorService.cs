using ZhmApi.Data;
using ZhmApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ZhmApi.Services
{
    public class TwoFactorService
    {
        private readonly ApiContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public TwoFactorService(ApiContext db, IEmailSender emailSender, IConfiguration config)
        {
            _db = db;
            _emailSender = emailSender;
            _config = config;
        }

        public async Task<int> CreateAndSendCodeAsync(int userId, string email)
        {
            var code = GenerateNumericCode(6);
            var codeHash = HashCode(code, userId);

            var twoFactorCode = new TwoFactorCode
            {
                UserId = userId,
                CodeHash = codeHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                Used = false,
                ResendCount = 0,
                LastSentAt = DateTime.UtcNow
            };

            _db.TwoFactorCodes.Add(twoFactorCode);
            await _db.SaveChangesAsync();

            await _emailSender.SendAsync(email, "Je verificatiecode", $"Je verificatiecode is: {code}\n\nDeze code is 10 minuten geldig.");

            return twoFactorCode.Id;
        }

        public async Task<(bool success, string reason)> VerifyCodeAsync(int sessionId, string code)
        {
            var now = DateTime.UtcNow;

            var entry = await _db.TwoFactorCodes.FindAsync(sessionId);
            if (entry == null)
                return (false, "no_session");

            if (entry.Used)
                return (false, "already_used");

            if (entry.ExpiresAt < now)
                return (false, "expired");

            var hash = HashCode(code, entry.UserId);
            if (!SecureEquals(hash, entry.CodeHash))
                return (false, "invalid_code");

            entry.Used = true;
            await _db.SaveChangesAsync();

            return (true, "");
        }

        public async Task<(bool success, string reason)> ResendAsync(int sessionId)
        {
            var entry = await _db.TwoFactorCodes
                .Include(tc => tc.User)
                .FirstOrDefaultAsync(tc => tc.Id == sessionId);
                
            if (entry == null) return (false, "no_session");

            if (entry.ResendCount >= 3)
                return (false, "too_many_resends");

            if (entry.Used)
                return (false, "already_used");

            if (entry.ExpiresAt < DateTime.UtcNow)
                return (false, "expired");

            entry.ResendCount++;
            entry.LastSentAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _emailSender.SendAsync(entry.User.Email!, "Je verificatiecode (opnieuw verzonden)", 
                "Je verificatiecode is opnieuw verzonden. Controleer je inbox voor de oorspronkelijke code.\n\nDeze code is nog steeds geldig.");

            return (true, "");
        }

        // Utility methods
        private string HashCode(string code, int userId)
        {
            var secret = _config["TWO_FACTOR_SECRET"] ?? "default-secret-key-change-in-production";
            using var hmac = new HMACSHA256(
                System.Text.Encoding.UTF8.GetBytes(secret + userId)
            );
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(hash);
        }

        private bool SecureEquals(string a, string b)
        {
            try
            {
                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromBase64String(a),
                    Convert.FromBase64String(b)
                );
            }
            catch
            {
                return false;
            }
        }

        private string GenerateNumericCode(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return string.Join("", bytes.Select(b => (b % 10).ToString()));
        }
    }
}
