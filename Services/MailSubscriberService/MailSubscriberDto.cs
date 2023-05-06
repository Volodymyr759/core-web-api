using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class MailSubscriberDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(50)]
        public string Email { get; set; }

        public bool IsSubscribed { get; set; } = false;

        [Required]
        public int MailSubscriptionId { get; set; }

        public MailSubscriptionDto MailSubscriptionDto { get; set; }
    }
}
