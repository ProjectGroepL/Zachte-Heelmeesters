using ZhmApi.Services;
using ZhmApi.Models;

namespace ZhmApi.Tests.POM
{
    public class AccessRequestPom
    {
        private readonly AccessRequestService _service;

        public AccessRequestPom(AccessRequestService service)
        {
            _service = service;
        }

        public Task<AccessRequest> RequestAccess(
            int specialistId,
            int appointmentId,
            string reason
        ) => _service.RequestAccess(specialistId, appointmentId, reason);

        public Task Decide(int requestId, int patientId, bool approved)
            => _service.DecideRequest(requestId, patientId, approved);

        public Task Revoke(int requestId, int patientId)
            => _service.RevokeAccess(requestId, patientId);
    }
}
