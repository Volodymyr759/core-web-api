using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
