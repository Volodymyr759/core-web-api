using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CoreWebApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // enable google smtp - https://support.google.com/accounts/answer/185833?authuser=1
            MailAddress mailFrom = new MailAddress(configuration["EmailSettings:EmailAddress"], "noreplay");
            MailAddress mailTo = new MailAddress(to);

            MailMessage msg = new MailMessage(mailFrom, mailTo)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient(configuration["EmailSettings:SmtpServer"], int.Parse(configuration["EmailSettings:SmtpPort"]))
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(configuration["EmailSettings:SmtpUser"], configuration["EmailSettings:SmtpKey"])
            };

            await client.SendMailAsync(msg);
        }
    }
}
