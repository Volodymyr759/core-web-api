using System.Threading.Tasks;
using System.Net.Mail;

namespace CoreWebApi.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            MailAddress mailFrom = new MailAddress("noreplay@seer.com", "noreplay");//"noreplay@old.housl.propertyspace.com", "apply@propertyspace.com" "noreplay"
            MailAddress mailTo = new MailAddress(to);

            MailMessage msg = new MailMessage(mailFrom, mailTo)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient("mail.smtp2go.com", 2525)
            {
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                EnableSsl = true,
                //client.UseDefaultCredentials = false;
                Credentials = new System.Net.NetworkCredential("cyberchisel.com", "y2Q5GXHkxYZQ")//Yahoo: "logisticmaster.2000@yahoo.com", "Adscasc21351-" 772#xDMF. "no-reply@propertyspace.com", "wshrxCEnQAxAEB9B"
            };//Google: "smtp.gmail.com", 587 | Yahoo "smtp.mail.yahoo.com", 587 smtp.old.housl.propertyspace.com "mail.propertyspace.com", 587

            await client.SendMailAsync(msg);
        }
    }
}
