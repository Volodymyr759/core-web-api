using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CoreWebApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration Configuration;

        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            MailAddress mailFrom = new MailAddress(Configuration["EmailSettings:EmailAddress"], "noreplay");
            MailAddress mailTo = new MailAddress(to);

            MailMessage msg = new MailMessage(mailFrom, mailTo)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient(Configuration["EmailSettings:SmtpServer"], 2525)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(Configuration["EmailSettings:SmtpUser"], Configuration["EmailSettings:SmtpKey"])
            };

            await client.SendMailAsync(msg);
        }
    }
}
