using System.Threading.Tasks;

namespace ZhmApi.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendAsync(string to, string subject, string body)
        {
            Console.WriteLine($"EMAIL → {to}: {subject}\n{body}");
            return Task.CompletedTask;
        }
    }
}
