namespace zhmApi.ServicesClasses 
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

    public async Task Request2FaAsync(int userId)
    {
        var code = GenerateNumericCode(6);
        var codeHash = HashCode(code, userId);

        var t = new TwoFactorCode
        {
            UserId = userId,
            CodeHash = codeHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            ResendCount = 0,
            LastSentAt = DateTime.UtcNow
        };

        _db.TwoFactorCodes.Add(t);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);

        await _emailSender.SendAsync(
            user.Email,
            "Je verificatiecode",
            $"Je code is: {code} — geldig tot {t.ExpiresAt:HH:mm}"
        );

        await LogEvent(userId, "2FARequested", new { codeLength = code.Length });
    }

    public async Task<VerifyResult> VerifyAsync(int userId, string code)
    {
        var now = DateTime.UtcNow;

        var entry = await _db.TwoFactorCodes
            .Where(c => c.UserId == userId && !c.Used)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (entry == null)
            return VerifyResult.NotFound;

        if (entry.ExpiresAt < now)
            return VerifyResult.Expired;

        var hash = HashCode(code, userId);
        if (!SecureEquals(hash, entry.CodeHash))
        {
            await LogEvent(userId, "2FAVerifyFailed", new { attemptedAt = now });
            return VerifyResult.Invalid;
        }

        entry.Used = true;
        await _db.SaveChangesAsync();

        await LogEvent(userId, "2FAVerifySuccess", null);

        return VerifyResult.Success;
    }

    public async Task ResendAsync(int userId)
    {
        var last = await _db.TwoFactorCodes
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (last != null &&
            last.ResendCount >= 3 &&
            last.LastSentAt > DateTime.UtcNow.AddHours(-1))
        {
            throw new InvalidOperationException("Resend limit reached");
        }

        await Request2FaAsync(userId);
    }
}
}