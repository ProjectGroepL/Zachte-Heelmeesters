using ZhmApi.Models;
using ZhmApi.Data;

namespace ZhmApi.Tests.Infrastructure
{
    

public static class TestDataBuilder
{
    public static AccessRequest CreatePendingRequest(ApiContext context)
    {
        var referral = new Referral
        {
            PatientId = 10,
            Status = ReferralStatus.Open
        };

        var appointment = new Appointment
        {
            SpecialistId = 5,
            Referral = referral,
            Status = AppointmentStatus.PendingAccess
        };

        var request = new AccessRequest
        {
            SpecialistId = 5,
            PatientId = 10,
            Appointment = appointment,
            Status = AccessRequestStatus.Pending
        };

        context.AddRange(referral, appointment, request);
        context.SaveChanges();

        return request;
    }

    public static AccessRequest CreateApprovedRequest(ApiContext context)
    {
        var request = CreatePendingRequest(context);
        request.Status = AccessRequestStatus.Approved;
        context.SaveChanges();
        return request;
    }
}
}
