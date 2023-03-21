using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class MailMessageDto
    {
        [Required(ErrorMessage = "Full name (1-50 characters) is required."), StringLength(50)]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "Email (1-50 characters) is required."), StringLength(50), EmailAddress]
        public string SenderEmail { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
}
