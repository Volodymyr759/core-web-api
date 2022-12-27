using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services.MailSubscriptionService
{
    public class MailSubscriptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title (1-100 characters) is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
    }
}
