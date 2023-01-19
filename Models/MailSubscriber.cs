using System;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models
{
    public class MailSubscriber
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(50)]
        public string Email { get; set; }

        public Boolean IsSubscribed { get; set; } = false;

        [Required]
        public int MailSubscriptionId { get; set; }
        public virtual MailSubscription MailSubscription { get; set; }
    }
}
