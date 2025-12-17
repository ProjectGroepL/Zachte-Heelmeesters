namespace ZhmApi.Dtos
{
    public class CreateReferralRequest
    {
        // Client sends camelCase JSON; model binding is case-insensitive.
        // Use consistent PascalCase property names for clarity in C# code.
        public int PatientId { get; set; }
        public int TreatmentId { get; set; }

        // Optional notes from the referring doctor
        public string? Notes { get; set; }

        // Optional initial status (defaults to 'open')
        public string? Status { get; set; }
    }
}